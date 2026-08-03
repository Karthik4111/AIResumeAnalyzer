using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Domain.Entities;
using AIResumeAnalyzer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzer.Infrastructure.Repositories;

public class InterviewQuestionRepository : IInterviewQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public InterviewQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(InterviewQuestion interviewQuestion)
    {
        await _context.InterviewQuestions.AddAsync(interviewQuestion);
    }

    public async Task<List<InterviewQuestion>> GetByATSReportIdAsync(Guid atsReportId)
    {
        return await _context.InterviewQuestions
            .Where(x => x.ATSReportId == atsReportId)
            .OrderBy(x => x.CreatedOnUtc)
            .ToListAsync();
    }
}