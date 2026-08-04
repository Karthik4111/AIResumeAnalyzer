using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Application.DTOs.Dashboard;

namespace AIResumeAnalyzer.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(Guid userId);
}
