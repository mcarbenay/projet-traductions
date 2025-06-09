using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Services;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace UseTheOps.PolyglotInitiative.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(UserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", request.Email);
                var user = await _userService.GetByEmailAsync(request.Email);
                if (user == null || !UserService.VerifyPassword(user, request.Password) || user.Status != "confirmed")
                {
                    _logger.LogWarning("Unauthorized login attempt for email: {Email}", request.Email);
                    return Unauthorized();
                }

                var token = GenerateJwtToken(user);
                Response.Cookies.Append("jwt", token, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(12)
                });
                _logger.LogInformation("User {UserId} logged in successfully", user.Id);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during login for email: {Email}", request?.Email);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "dev_secret_key_please_change";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            };
            // Add role claims (normalize to PascalCase)
            if (user.IsAdministrator)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else if (user.UserSolutionAccesses != null && user.UserSolutionAccesses.Count > 0)
            {
                foreach (var role in user.UserSolutionAccesses.Select(a => a.AccessLevel).Distinct())
                {
                    string normalizedRole = role.ToLower();
                    if (normalizedRole == "product_owner") normalizedRole = "ProductOwner";
                    else if (normalizedRole == "translator") normalizedRole = "Translator";
                    else if (normalizedRole == "reader") normalizedRole = "Reader";
                    else normalizedRole = role;
                    claims.Add(new Claim(ClaimTypes.Role, normalizedRole));
                }
            }
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("login/apikey")]
        public async Task<IActionResult> LoginApiKey([FromBody] ApiKeyLoginRequest request, [FromServices] ApiKeyService apiKeyService)
        {
            _logger.LogInformation("API Key login attempt with key: {ApiKey}", request.ApiKey);
            var apiKey = (await apiKeyService.GetAllAsync()).FirstOrDefault(k => k.KeyValue == request.ApiKey && k.RevokedAt == null);
            if (apiKey == null)
            {
                _logger.LogWarning("Unauthorized API Key login attempt with key: {ApiKey}", request.ApiKey);
                return Unauthorized();
            }
            var token = GenerateApiKeyJwtToken(apiKey);
            Response.Cookies.Append("jwt", token, new Microsoft.AspNetCore.Http.CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(12)
            });
            _logger.LogInformation("API Key login successful, key ID: {ApiKeyId}", apiKey.Id);
            return Ok(new { token });
        }

        private string GenerateApiKeyJwtToken(ApiKey apiKey)
        {
            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "dev_secret_key_please_change";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim("api_key_id", apiKey.Id.ToString()),
                new Claim("solution_id", apiKey.SolutionId.ToString()),
                new Claim(ClaimTypes.Role, "ProductOwner")
            };
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class ApiKeyLoginRequest
        {
            public string ApiKey { get; set; } = string.Empty;
        }
    }
}
