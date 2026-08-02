using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Domain.Entities;
using AIResumeAnalyzer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzer.Infrastructure.Repositories;

public class ResumeVersionRepository: GenericRepository<ResumeVersion>, IResumeVersionRepository
{
    public ResumeVersionRepository(ApplicationDbContext context): base(context)
    {
    }

    public async Task<int> GetLatestVersionNumberAsync(Guid resumeId)
    {
        var latestVersion = await _context.ResumeVersions
            .Where(x => x.ResumeId == resumeId)
            .OrderByDescending(x => x.VersionNumber)
            .Select(x => (int?)x.VersionNumber)
            .FirstOrDefaultAsync();

        return latestVersion ?? 0;
    }

    public async Task<ResumeVersion?> GetLatestVersionAsync(Guid resumeId)
    {
        return await _context.ResumeVersions
            .Where(v => v.ResumeId == resumeId)
            .OrderByDescending(v => v.VersionNumber)
            .FirstOrDefaultAsync();
    }
}