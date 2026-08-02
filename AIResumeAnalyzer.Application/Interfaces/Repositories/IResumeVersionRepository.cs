using AIResumeAnalyzer.Domain.Entities;

namespace AIResumeAnalyzer.Application.Interfaces.Repositories;

public interface IResumeVersionRepository
    : IGenericRepository<ResumeVersion>
{
    Task<int> GetLatestVersionNumberAsync(Guid resumeId);
}