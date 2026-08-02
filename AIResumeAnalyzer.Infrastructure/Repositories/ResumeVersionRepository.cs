using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Domain.Entities;
using AIResumeAnalyzer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzer.Infrastructure.Repositories;

public class ResumeVersionRepository: GenericRepository<ResumeVersion>, IResumeVersionRepository
{
    public ResumeVersionRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<int> GetLatestVersionNumberAsync(Guid resumeId)
    {
        return await _context.ResumeVersions
            .Where(x => x.ResumeId == resumeId)
            .Select(x => x.VersionNumber)
            .DefaultIfEmpty(0)
            .MaxAsync();
    }
}