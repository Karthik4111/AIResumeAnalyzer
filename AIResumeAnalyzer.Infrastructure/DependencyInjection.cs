using AIResumeAnalyzer.Application.Interfaces.AI;
using AIResumeAnalyzer.Application.Interfaces.ATS;
using AIResumeAnalyzer.Application.Interfaces.Persistence;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Application.Interfaces.ResumeParsing;
using AIResumeAnalyzer.Application.Interfaces.Services;
using AIResumeAnalyzer.Application.Interfaces.Services;
using AIResumeAnalyzer.Infrastructure.Persistence;
using AIResumeAnalyzer.Infrastructure.Repositories;
using AIResumeAnalyzer.Infrastructure.Services.Admin;
using AIResumeAnalyzer.Infrastructure.Services.AI;
using AIResumeAnalyzer.Infrastructure.Services.ATS;
using AIResumeAnalyzer.Infrastructure.Services.CoverLetter;
using AIResumeAnalyzer.Infrastructure.Services.Dashboard;
using AIResumeAnalyzer.Infrastructure.Services.File;
using AIResumeAnalyzer.Infrastructure.Services.JobDescription;
using AIResumeAnalyzer.Infrastructure.Services.Recommendation;
using AIResumeAnalyzer.Infrastructure.Services.Resume;
using AIResumeAnalyzer.Infrastructure.Services.ResumeParsing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AIResumeAnalyzer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IResumeService, ResumeService>();

        services.AddScoped<IResumeRepository, ResumeRepository>();

        services.AddScoped<IResumeVersionRepository, ResumeVersionRepository>();

        services.AddScoped<IResumeParserService, ResumeParserService>();

        services.AddScoped<IATSService, ATSService>();

        services.AddScoped<IATSReportRepository, ATSReportRepository>();

        services.AddScoped<IJobDescriptionRepository, JobDescriptionRepository>();

        services.AddScoped<IJobDescriptionService, JobDescriptionService>();

        services.AddHttpClient<IAIService, OllamaService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:11434/");
        });

        services.AddScoped<IInterviewQuestionRepository, InterviewQuestionRepository>();

        services.AddScoped<ICoverLetterRepository, CoverLetterRepository>();

        services.AddHttpClient<ICoverLetterService, CoverLetterService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:11434/");
        });

        services.AddScoped<IRecommendationRepository, RecommendationRepository>();

        services.AddHttpClient<IRecommendationService, RecommendationService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:11434/");
        });

        services.AddScoped<IDashboardRepository, DashboardRepository>();

        services.AddScoped<IDashboardService, DashboardService>();

        services.AddScoped<IAdminRepository, AdminRepository>();

        services.AddScoped<IAdminRepository, AdminRepository>();

        services.AddScoped<IAdminService, AdminService>();

        services.AddScoped<IFileRepository, FileRepository>();

        services.AddScoped<IFileService, FileService>();

        return services;
    }
}