using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Domain.Entities;
using AIResumeAnalyzer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzer.Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    private readonly ApplicationDbContext _context;

    public FileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Resume?> GetResumeWithVersionsAsync(Guid resumeId)
    {
        return await _context.Resumes
            .Include(x => x.Versions)
            .FirstOrDefaultAsync(x => x.Id == resumeId);
    }

    public async Task<ResumeVersion?> GetVersionAsync(Guid versionId)
    {
        return await _context.ResumeVersions
            .Include(x => x.Resume)
            .FirstOrDefaultAsync(x => x.Id == versionId);
    }

    public async Task DeleteResumeAsync(Guid resumeId)
    {
        var resume = await _context.Resumes
            .Include(x => x.Versions)
            .FirstOrDefaultAsync(x => x.Id == resumeId);

        if (resume == null)
            throw new Exception("Resume not found.");

        resume.IsDeleted = true;
        resume.DeletedOnUtc = DateTime.UtcNow;

        foreach (var version in resume.Versions)
        {
            version.IsDeleted = true;
            version.DeletedOnUtc = DateTime.UtcNow;
        }
    }
}