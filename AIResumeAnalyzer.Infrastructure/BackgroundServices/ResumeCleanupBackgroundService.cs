using AIResumeAnalyzer.Application.Interfaces.Persistence;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AIResumeAnalyzer.Infrastructure.BackgroundServices;

public class ResumeCleanupBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ResumeCleanupBackgroundService> _logger;

    public ResumeCleanupBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<ResumeCleanupBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Resume Cleanup Background Service Started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var resumeRepository = scope.ServiceProvider
                    .GetRequiredService<IResumeRepository>();

                var unitOfWork = scope.ServiceProvider
                    .GetRequiredService<IUnitOfWork>();

                var expiredResumes =
                    await resumeRepository.GetExpiredSoftDeletedResumesAsync(30);

                _logger.LogInformation(
                    "Found {Count} expired resumes.",
                    expiredResumes.Count);

                foreach (var resume in expiredResumes)
                {
                    foreach (var version in resume.Versions)
                    {
                        try
                        {
                            if (File.Exists(version.FilePath))
                            {
                                File.Delete(version.FilePath);

                                _logger.LogInformation(
                                    "Deleted resume file: {FileName}",
                                    version.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(
                                ex,
                                "Failed to delete resume file: {FileName}",
                                version.FileName);
                        }
                    }

                    resumeRepository.Delete(resume);
                }

                if (expiredResumes.Any())
                {
                    await unitOfWork.SaveChangesAsync();

                    _logger.LogInformation(
                        "Deleted {Count} expired resumes successfully.",
                        expiredResumes.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while executing Resume Cleanup Background Service.");
            }

            await Task.Delay(
                TimeSpan.FromMinutes(1),
                stoppingToken);
        }

        _logger.LogInformation("Resume Cleanup Background Service Stopped.");
    }
}