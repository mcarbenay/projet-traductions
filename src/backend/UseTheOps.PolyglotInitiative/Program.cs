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
            });
            builder.Services.AddDbContext<PolyglotInitiativeDbContext>(options =>
                options.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]));
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
            // TODO: Add authentication, authorization, OpenTelemetry, background services, etc.

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
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
