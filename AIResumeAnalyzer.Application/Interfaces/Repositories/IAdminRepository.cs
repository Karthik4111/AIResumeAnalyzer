using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Application.DTOs.Admin;

namespace AIResumeAnalyzer.Application.Interfaces.Repositories;

public interface IAdminRepository
{
    Task<AdminDashboardResponse> GetDashboardAsync();

    Task<List<UserResponse>> GetUsersAsync();

    Task DeleteUserAsync(Guid userId);
}