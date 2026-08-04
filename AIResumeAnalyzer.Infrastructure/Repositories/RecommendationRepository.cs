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

public class RecommendationRepository
    : GenericRepository<Recommendation>, IRecommendationRepository
{
    private readonly ApplicationDbContext _context;

    public RecommendationRepository(ApplicationDbContext context): base(context)
    {
        _context = context;
    }

    public async Task AddAsync(Recommendation recommendation)
    {
        await _context.Recommendations.AddAsync(recommendation);
    }

    public async Task<List<Recommendation>> GetByResumeIdAsync(Guid resumeId)
    {
        return await _context.Recommendations
            .Include(x => x.ATSReport)
                .ThenInclude(x => x.ResumeVersion)
            .Where(x => x.ATSReport.ResumeVersion.ResumeId == resumeId)
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToListAsync();
    }

    public async Task<Recommendation?> GetByIdAsync(Guid id)
    {
        return await _context.Recommendations
            .Include(x => x.ATSReport)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
