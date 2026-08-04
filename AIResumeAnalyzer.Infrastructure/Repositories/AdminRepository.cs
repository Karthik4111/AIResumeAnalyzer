using AIResumeAnalyzer.Application.DTOs.Admin;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzer.Infrastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly ApplicationDbContext _context;

    public AdminRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardResponse> GetDashboardAsync()
    {
        return new AdminDashboardResponse
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalResumes = await _context.Resumes.CountAsync(),
            TotalJobDescriptions = await _context.JobDescriptions.CountAsync(),
            TotalATSReports = await _context.ATSReports.CountAsync(),
            TotalCoverLetters = await _context.CoverLetters.CountAsync(),
            TotalInterviewQuestions = await _context.InterviewQuestions.CountAsync(),
            TotalRecommendations = await _context.Recommendations.CountAsync()
        };
    }

    public async Task<List<UserResponse>> GetUsersAsync()
    {
        return await _context.Users
            .Include(x => x.Role)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                Name = x.FirstName + " " + x.LastName,
                Email = x.Email,
                Role = x.Role.Name
            })
            .ToListAsync();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            throw new Exception("User not found.");

        user.IsDeleted = true;
        user.DeletedOnUtc = DateTime.UtcNow;
    }
}