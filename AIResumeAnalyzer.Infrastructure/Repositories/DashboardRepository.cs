using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Application.DTOs.Dashboard;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzer.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardResponse> GetDashboardAsync(Guid userId)
    {
        var totalResumes = await _context.Resumes
            .CountAsync(x => x.UserId == userId);

        var totalJobDescriptions = await _context.JobDescriptions
            .CountAsync(x => x.UserId == userId);

        var totalATSReports = await _context.ATSReports
            .Include(x => x.ResumeVersion)
            .Where(x => x.ResumeVersion.Resume.UserId == userId)
            .CountAsync();

        var totalCoverLetters = await _context.CoverLetters
            .Include(x => x.ATSReport)
            .ThenInclude(x => x.ResumeVersion)
            .Where(x => x.ATSReport.ResumeVersion.Resume.UserId == userId)
            .CountAsync();

        var totalInterviewQuestions = await _context.InterviewQuestions
            .Include(x => x.ATSReport)
            .ThenInclude(x => x.ResumeVersion)
            .Where(x => x.ATSReport.ResumeVersion.Resume.UserId == userId)
            .CountAsync();

        var totalRecommendations = await _context.Recommendations
            .Include(x => x.ATSReport)
            .ThenInclude(x => x.ResumeVersion)
            .Where(x => x.ATSReport.ResumeVersion.Resume.UserId == userId)
            .CountAsync();

        var reports = await _context.ATSReports
            .Include(x => x.ResumeVersion)
            .Where(x => x.ResumeVersion.Resume.UserId == userId)
            .ToListAsync();

        double averageScore = reports.Any()
        ? (double)reports.Average(x => x.AtsScore)
        : 0;

        int highestScore = reports.Any()
            ? (int)reports.Max(x => x.AtsScore)
            : 0;

        return new DashboardResponse
        {
            TotalResumes = totalResumes,
            TotalJobDescriptions = totalJobDescriptions,
            TotalATSReports = totalATSReports,
            TotalCoverLetters = totalCoverLetters,
            TotalInterviewQuestions = totalInterviewQuestions,
            TotalRecommendations = totalRecommendations,
            AverageATSScore = Math.Round(averageScore, 2),
            HighestATSScore = highestScore
        };
    }
}