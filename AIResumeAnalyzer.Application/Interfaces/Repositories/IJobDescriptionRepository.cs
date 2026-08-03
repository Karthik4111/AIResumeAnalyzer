using AIResumeAnalyzer.Domain.Entities;

namespace AIResumeAnalyzer.Application.Interfaces.Repositories;

public interface IJobDescriptionRepository: IGenericRepository<JobDescription>
{
    Task<List<JobDescription>> GetAllAsync();

    Task<JobDescription?> GetByIdAsync(Guid id);
}