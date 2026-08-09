using AIResumeAnalyzer.API.Middleware;
using AIResumeAnalyzer.Application;
using AIResumeAnalyzer.Application.Interfaces.Auth;
using AIResumeAnalyzer.Domain.Common;
using AIResumeAnalyzer.Infrastructure;
using AIResumeAnalyzer.Infrastructure.Persistence;
using AIResumeAnalyzer.Infrastructure.Services.Auth;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Serilog;

using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

namespace AIResumeAnalyzer.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ============================================================
            // Serilog
            // ============================================================

            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(
                        Path.Combine(
                            AppContext.BaseDirectory,
                            "Logs",
                            "log-.txt"),
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 30,
                        shared: true);
            });

            // ============================================================
            // Controllers
            // ============================================================

            builder.Services.AddControllers();

            // ============================================================
            // CORS
            // Allow React frontend to communicate with the API
            // ============================================================

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ReactFrontend", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // ============================================================
            // Memory Cache
            // ============================================================

            builder.Services.AddMemoryCache();

            // ============================================================
            // Rate Limiting
            // ============================================================

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode =
                    StatusCodes.Status429TooManyRequests;

                options.AddFixedWindowLimiter(
                    "LoginPolicy",
                    limiterOptions =>
                    {
                        limiterOptions.PermitLimit = 5;
                        limiterOptions.Window =
                            TimeSpan.FromMinutes(1);

                        limiterOptions.QueueProcessingOrder =
                            QueueProcessingOrder.OldestFirst;

                        limiterOptions.QueueLimit = 0;
                    });

                options.AddFixedWindowLimiter(
                    "DefaultPolicy",
                    limiterOptions =>
                    {
                        limiterOptions.PermitLimit = 100;
                        limiterOptions.Window =
                            TimeSpan.FromMinutes(1);

                        limiterOptions.QueueProcessingOrder =
                            QueueProcessingOrder.OldestFirst;

                        limiterOptions.QueueLimit = 10;
                    });
            });

            // ============================================================
            // Health Checks
            // ============================================================

            builder.Services
                .AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>();

            // ============================================================
            // Swagger
            // ============================================================

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "AI Resume Analyzer API",
                        Version = "v1"
                    });

                var jwtSecurityScheme =
                    new OpenApiSecurityScheme
                    {
                        BearerFormat = "JWT",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        Description = "Enter JWT Bearer token",

                        Reference =
                            new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                    };

                options.AddSecurityDefinition(
                    "Bearer",
                    jwtSecurityScheme);

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            jwtSecurityScheme,
                            Array.Empty<string>()
                        }
                    });
            });

            // ============================================================
            // Application & Infrastructure
            // ============================================================

            builder.Services.AddApplication();

            builder.Services.AddInfrastructure(
                builder.Configuration);

            // ============================================================
            // JWT Configuration
            // ============================================================

            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection(
                    JwtOptions.SectionName));

            // ============================================================
            // Authentication Services
            // ============================================================

            builder.Services.AddScoped<
                IJwtTokenGenerator,
                JwtTokenGenerator>();

            builder.Services.AddScoped<
                IPasswordHasher,
                PasswordHasher>();

            builder.Services.AddScoped<
                IAuthService,
                AuthService>();

            // ============================================================
            // JWT Authentication
            // ============================================================

            builder.Services
                .AddAuthentication(
                    JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwt = builder.Configuration
                        .GetSection(JwtOptions.SectionName)
                        .Get<JwtOptions>()!;

                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = jwt.Issuer,
                            ValidAudience = jwt.Audience,

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        jwt.SecretKey)),

                            ClockSkew = TimeSpan.Zero,

                            NameClaimType =
                                ClaimTypes.Name,

                            RoleClaimType =
                                ClaimTypes.Role
                        };
                });

            // ============================================================
            // Authorization
            // ============================================================

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "AdminOnly",
                    policy =>
                        policy.RequireRole("Admin"));

                options.AddPolicy(
                    "CandidateOnly",
                    policy =>
                        policy.RequireRole("Candidate"));

                options.AddPolicy(
                    "RecruiterOnly",
                    policy =>
                        policy.RequireRole("Recruiter"));
            });

            // ============================================================
            // Build Application
            // ============================================================

            var app = builder.Build();

            // ============================================================
            // Swagger
            // ============================================================

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ============================================================
            // Middleware Pipeline
            // ============================================================

            app.UseSerilogRequestLogging();

            app.UseRequestLogging();

            app.UseGlobalExceptionHandler();

            app.UseHttpsRedirection();

            // IMPORTANT:
            // Must be before Authentication / Authorization
            app.UseCors("ReactFrontend");

            app.UseRateLimiter();

            app.UseAuthentication();

            app.UseAuthorization();

            // ============================================================
            // Controllers
            // ============================================================

            app.MapControllers();

            // ============================================================
            // Health Check
            // ============================================================

            app.MapHealthChecks("/health");

            // ============================================================
            // Application Started
            // ============================================================

            Log.Information(
                "AI Resume Analyzer API started successfully.");

            app.Run();
        }
    }
}