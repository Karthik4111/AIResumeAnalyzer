using AIResumeAnalyzer.Domain.Entities;

namespace AIResumeAnalyzer.Application.Interfaces.Repositories;

public interface IResumeRepository : IGenericRepository<Resume>
{
    Task<List<Resume>> GetByUserIdAsync(Guid userId);

    Task<Resume?> GetByIdWithVersionsAsync(Guid resumeId);

    Task<List<Resume>> GetExpiredSoftDeletedResumesAsync(int days);

    void Delete(Resume resume);
}