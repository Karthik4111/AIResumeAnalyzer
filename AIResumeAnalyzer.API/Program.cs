using AIResumeAnalyzer.Application;
using AIResumeAnalyzer.Application.Interfaces.Auth;
using AIResumeAnalyzer.Domain.Common;
using AIResumeAnalyzer.Infrastructure;
using AIResumeAnalyzer.Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace AIResumeAnalyzer.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Controllers
            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AI Resume Analyzer API",
                    Version = "v1"
                });

                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Description = "Enter JWT Bearer token",

                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.AddSecurityDefinition("Bearer", jwtSecurityScheme);

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        jwtSecurityScheme,
                        Array.Empty<string>()
                    }
                });
            });

            // Register Application & Infrastructure
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            // JWT Configuration
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection(JwtOptions.SectionName));

            // DEBUG - Verify JWT Configuration
            var jwtConfig = builder.Configuration
                .GetSection(JwtOptions.SectionName)
                .Get<JwtOptions>();

            Console.WriteLine("======================================");
            Console.WriteLine("JWT Configuration");
            Console.WriteLine("======================================");
            Console.WriteLine($"Issuer     : {jwtConfig?.Issuer}");
            Console.WriteLine($"Audience   : {jwtConfig?.Audience}");
            Console.WriteLine($"Secret Key : {jwtConfig?.SecretKey}");
            Console.WriteLine($"Expiry     : {jwtConfig?.ExpiryMinutes}");
            Console.WriteLine("======================================");

            // Dependency Injection
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            // JWT Authentication
            builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()!;

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt.SecretKey)),

            ClockSkew = TimeSpan.Zero,

            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

            // Authorization Policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("CandidateOnly", policy =>
                    policy.RequireRole("Candidate"));

                options.AddPolicy("RecruiterOnly", policy =>
                    policy.RequireRole("Recruiter"));
            });

            var app = builder.Build();

            // Configure HTTP Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}