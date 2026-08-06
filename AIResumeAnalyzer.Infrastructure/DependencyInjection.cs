using AIResumeAnalyzer.Application.Configuration;
using AIResumeAnalyzer.Application.Interfaces.AI;
using AIResumeAnalyzer.Application.Interfaces.ATS;
using AIResumeAnalyzer.Application.Interfaces.Common;
using AIResumeAnalyzer.Application.Interfaces.Persistence;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Application.Interfaces.ResumeParsing;
using AIResumeAnalyzer.Application.Interfaces.Services;
using AIResumeAnalyzer.Infrastructure.BackgroundServices;
using AIResumeAnalyzer.Infrastructure.Persistence;
using AIResumeAnalyzer.Infrastructure.Repositories;
using AIResumeAnalyzer.Infrastructure.Services.Admin;
using AIResumeAnalyzer.Infrastructure.Services.AI;
using AIResumeAnalyzer.Infrastructure.Services.ATS;
using AIResumeAnalyzer.Infrastructure.Services.Cache;
using AIResumeAnalyzer.Infrastructure.Services.CoverLetter;
using AIResumeAnalyzer.Infrastructure.Services.Dashboard;
using AIResumeAnalyzer.Infrastructure.Services.Email;
using AIResumeAnalyzer.Infrastructure.Services.File;
using AIResumeAnalyzer.Infrastructure.Services.JobDescription;
using AIResumeAnalyzer.Infrastructure.Services.Logging;
using AIResumeAnalyzer.Infrastructure.Services.Pdf;
using AIResumeAnalyzer.Infrastructure.Services.Recommendation;
using AIResumeAnalyzer.Infrastructure.Services.Resume;
using AIResumeAnalyzer.Infrastructure.Services.ResumeParsing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AIResumeAnalyzer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));
        });

        // Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Logger
        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IResumeRepository, ResumeRepository>();
        services.AddScoped<IResumeVersionRepository, ResumeVersionRepository>();
        services.AddScoped<IATSReportRepository, ATSReportRepository>();
        services.AddScoped<IJobDescriptionRepository, JobDescriptionRepository>();
        services.AddScoped<IInterviewQuestionRepository, InterviewQuestionRepository>();
        services.AddScoped<ICoverLetterRepository, CoverLetterRepository>();
        services.AddScoped<IRecommendationRepository, RecommendationRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IFileRepository, FileRepository>();

        // Services
        services.AddScoped<IResumeService, ResumeService>();
        services.AddScoped<IResumeParserService, ResumeParserService>();
        services.AddScoped<IATSService, ATSService>();
        services.AddScoped<IJobDescriptionService, JobDescriptionService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IPdfService, PdfService>();
        services.AddScoped<ICacheService, MemoryCacheService>();

        // Email
        services.Configure<EmailSettings>(
            configuration.GetSection("EmailSettings"));

        services.AddScoped<IEmailService, EmailService>();

        // AI Services
        services.AddHttpClient<IAIService, OllamaService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:11434/");
        });

        services.AddHttpClient<ICoverLetterService, CoverLetterService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:11434/");
        });

        services.AddHttpClient<IRecommendationService, RecommendationService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:11434/");
        });

        // Background Services
        services.AddHostedService<ResumeCleanupBackgroundService>();

        return services;
    }
}