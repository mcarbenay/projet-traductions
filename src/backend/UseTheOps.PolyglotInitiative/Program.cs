using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Middleware;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;

namespace UseTheOps.PolyglotInitiative
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlFilename));
                // Add JWT Bearer security definition
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            // Use environment variable for connection string (no appsettings.json fallback)
            var pgConnStr = Environment.GetEnvironmentVariable("PG_CONNECTION_STRING")
                ?? throw new InvalidOperationException("PG_CONNECTION_STRING environment variable is required.");
            builder.Services.AddDbContext<PolyglotInitiativeDbContext>(options =>
                options.UseNpgsql(pgConnStr));
            builder.Services.AddLocalization();
            builder.Services.AddLogging();
            builder.Services.AddScoped<UseTheOps.PolyglotInitiative.Services.ProjectService>();
            builder.Services.AddScoped<UseTheOps.PolyglotInitiative.Services.SolutionService>();
            builder.Services.AddScoped<UseTheOps.PolyglotInitiative.Services.ComponentService>();
            builder.Services.AddScoped<UseTheOps.PolyglotInitiative.Services.ResourceFileService>();
            builder.Services.AddScoped<UseTheOps.PolyglotInitiative.Services.TranslatableResourceService>();
            builder.Services.AddScoped<UseTheOps.PolyglotInitiative.Services.ResourceTranslationService>();
            builder.Services.AddScoped<UseTheOps.PolyglotInitiative.Services.TranslationNeedService>();
            builder.Services.AddScoped<UseTheOps.PolyglotInitiative.Services.UserService>();
            builder.Services.AddScoped<UseTheOps.PolyglotInitiative.Services.ApiKeyService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<UseTheOps.PolyglotInitiative.Services.AuthorizationService>();
            // TODO: Add authentication, authorization, OpenTelemetry, background services, etc.
            // Add a default authentication scheme to avoid InvalidOperationException
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "SuperSecretKeyForJwtToken123456!@#";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ClockSkew = TimeSpan.FromMinutes(2)
                    };
                    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            // Allow JWT from cookie as well as Authorization header
                            if (ctx.Request.Cookies.ContainsKey("jwt"))
                                ctx.Token = ctx.Request.Cookies["jwt"];
                            return Task.CompletedTask;
                        }
                    };
                });
            // OpenTelemetry: Tracing, Metrics, Logging
            //builder.Services.AddOpenTelemetry()
            //    .WithTracing(tracing => tracing
            //        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PolyglotInitiative"))
            //        .AddAspNetCoreInstrumentation()
            //        .AddHttpClientInstrumentation()
            //        .AddEntityFrameworkCoreInstrumentation()
            //        .AddConsoleExporter() // Console exporter for dev
            //    )
            //    .WithMetrics(metrics => metrics
            //        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PolyglotInitiative"))
            //        .AddAspNetCoreInstrumentation()
            //        .AddHttpClientInstrumentation()
            //        .AddRuntimeInstrumentation()
            //        .AddConsoleExporter() // Console exporter for dev
            //    );
            builder.Logging.ClearProviders();
            builder.Logging.AddOpenTelemetry(options =>
            {
                options.IncludeScopes = true;
                options.ParseStateValues = true;
                options.IncludeFormattedMessage = true;
                options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PolyglotInitiative"));
                options.AddConsoleExporter();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<OpenTelemetryActivityMiddleware>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // Apply EF Core migrations at startup
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<PolyglotInitiativeDbContext>();
                db.Database.Migrate();
                // Insert default admin if no users exist
                if (!db.Users.Any())
                {
                    var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL");
                    var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
                    if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
                    {
                        var adminUser = new UseTheOps.PolyglotInitiative.Models.User
                        {
                            Name = "Admin",
                            Email = adminEmail,
                            PasswordHash = UseTheOps.PolyglotInitiative.Services.UserService.HashPassword(adminPassword),
                            IsAdministrator = true,
                            Status = "confirmed"
                        };
                        db.Users.Add(adminUser);
                        db.SaveChanges();
                    }
                }
            }

            app.Run();
        }
    }
}
