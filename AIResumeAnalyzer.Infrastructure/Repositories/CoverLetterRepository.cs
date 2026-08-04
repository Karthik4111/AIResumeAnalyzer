using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Domain.Entities;
using AIResumeAnalyzer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzer.Infrastructure.Repositories;

public class CoverLetterRepository: GenericRepository<CoverLetter>, ICoverLetterRepository
{
    private readonly ApplicationDbContext _context;

    public CoverLetterRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task AddAsync(CoverLetter coverLetter)
    {
        await _context.CoverLetters.AddAsync(coverLetter);
    }

    public async Task<List<CoverLetter>> GetByResumeIdAsync(Guid resumeId)
    {
        return await _context.CoverLetters
            .Include(x => x.ATSReport)
                .ThenInclude(x => x.ResumeVersion)
            .Where(x => x.ATSReport.ResumeVersion.ResumeId == resumeId)
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToListAsync();
    }

    public async Task<CoverLetter?> GetByIdAsync(Guid id)
    {
        return await _context.CoverLetters
            .Include(x => x.ATSReport)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}