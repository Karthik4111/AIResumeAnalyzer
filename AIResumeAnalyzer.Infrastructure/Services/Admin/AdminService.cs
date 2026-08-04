using AIResumeAnalyzer.Application.DTOs.Admin;
using AIResumeAnalyzer.Application.Interfaces.Persistence;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Application.Interfaces.Services;

namespace AIResumeAnalyzer.Infrastructure.Services.Admin;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(IAdminRepository adminRepository,IUnitOfWork unitOfWork)
    {
        _adminRepository = adminRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AdminDashboardResponse> GetDashboardAsync()
    {
        return await _adminRepository.GetDashboardAsync();
    }

    public async Task<List<UserResponse>> GetUsersAsync()
    {
        return await _adminRepository.GetUsersAsync();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        await _adminRepository.DeleteUserAsync(userId);

        await _unitOfWork.SaveChangesAsync();
    }
}