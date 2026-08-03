using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Domain.Entities;
using AIResumeAnalyzer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzer.Infrastructure.Repositories;

public class ATSReportRepository: GenericRepository<ATSReport>, IATSReportRepository
{
    private readonly ApplicationDbContext _context;

    public ATSReportRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<ATSReport>> GetByResumeIdAsync(Guid resumeId)
    {
        return await _context.ATSReports
            .Include(x => x.ResumeVersion)
            .Where(x => x.ResumeVersion.ResumeId == resumeId)
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToListAsync();
    }

    public async Task<ATSReport?> GetLatestByResumeIdAsync(Guid resumeId)
    {
        return await _context.ATSReports
            .Include(x => x.ResumeVersion)
            .Where(x => x.ResumeVersion.ResumeId == resumeId)
            .OrderByDescending(x => x.CreatedOnUtc)
            .FirstOrDefaultAsync();
    }
}