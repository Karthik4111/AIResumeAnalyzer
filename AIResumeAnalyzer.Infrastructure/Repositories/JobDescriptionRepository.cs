using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Domain.Entities;
using AIResumeAnalyzer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzer.Infrastructure.Repositories;

public class JobDescriptionRepository
    : GenericRepository<JobDescription>,
      IJobDescriptionRepository
{
    private readonly ApplicationDbContext _context;

    public JobDescriptionRepository(
        ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<List<JobDescription>> GetAllAsync()
    {
        return await _context.JobDescriptions
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToListAsync();
    }
}