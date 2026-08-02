using AIResumeAnalyzer.Domain.Entities;

namespace AIResumeAnalyzer.Application.Interfaces.Repositories;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByRoleNameAsync(string roleName);
}