using AIResumeAnalyzer.Domain.Entities;

namespace AIResumeAnalyzer.Application.Interfaces.Repositories;

public interface IResumeRepository : IGenericRepository<Resume>
{
    Task<List<Resume>> GetByUserIdAsync(Guid userId);

    Task<Resume?> GetByIdWithVersionsAsync(Guid resumeId);
}