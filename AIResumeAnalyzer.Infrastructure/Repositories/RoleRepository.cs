using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Domain.Entities;
using AIResumeAnalyzer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzer.Infrastructure.Repositories;

public class RoleRepository
    : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Role?> GetByRoleNameAsync(string roleName)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(x => x.Name == roleName);
    }
}