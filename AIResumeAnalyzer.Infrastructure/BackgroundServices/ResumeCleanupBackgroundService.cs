using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AIResumeAnalyzer.Infrastructure.BackgroundServices;

public class ResumeCleanupBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ResumeCleanupBackgroundService> _logger;

    public ResumeCleanupBackgroundService(IServiceScopeFactory scopeFactory,ILogger<ResumeCleanupBackgroundService> logger)
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

                _logger.LogInformation(
                    "Background Job Executed at {Time}",
                    DateTime.UtcNow);

                // Future cleanup logic goes here

                await Task.Delay(
                    TimeSpan.FromMinutes(1),
                    stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while executing background service.");
            }
        }

        _logger.LogInformation("Resume Cleanup Background Service Stopped.");
    }
}