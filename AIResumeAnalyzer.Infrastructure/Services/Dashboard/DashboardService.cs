using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Application.DTOs.Dashboard;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Application.Interfaces.Services;

namespace AIResumeAnalyzer.Infrastructure.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardResponse> GetDashboardAsync(Guid userId)
    {
        return await _dashboardRepository.GetDashboardAsync(userId);
    }
}