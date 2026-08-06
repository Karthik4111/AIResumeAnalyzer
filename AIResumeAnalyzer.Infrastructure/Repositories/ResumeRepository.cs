using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Domain.Entities;
using AIResumeAnalyzer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzer.Infrastructure.Repositories;

public class ResumeRepository
    : GenericRepository<Resume>, IResumeRepository
{
    public ResumeRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<List<Resume>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Resumes
            .Include(r => r.Versions)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedOnUtc)
            .ToListAsync();
    }

    public async Task<AIResumeAnalyzer.Domain.Entities.Resume?> GetByIdWithVersionsAsync(Guid resumeId)
    {
        return await _context.Resumes
            .Include(r => r.Versions)
            .FirstOrDefaultAsync(r => r.Id == resumeId);
    }

    public async Task<Resume?> GetByIdAsync(Guid resumeId)
    {
        return await _context.Resumes
            .FirstOrDefaultAsync(r => r.Id == resumeId);
    }

    public async Task<List<Resume>> GetExpiredSoftDeletedResumesAsync(int days)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);

        return await _context.Resumes
            .Where(r =>
                r.IsDeleted &&
                r.DeletedOnUtc != null &&
                r.DeletedOnUtc <= cutoffDate)
            .Include(r => r.Versions)
            .ToListAsync();
    }

    public void Delete(Resume resume)
    {
        _context.Resumes.Remove(resume);
    }
}