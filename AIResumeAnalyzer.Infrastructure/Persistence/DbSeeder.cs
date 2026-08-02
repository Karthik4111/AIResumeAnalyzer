using AIResumeAnalyzer.Domain.Entities;
using AIResumeAnalyzer.Domain.Enums;

namespace AIResumeAnalyzer.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        if (context.Roles.Any())
            return;

        var roles = new List<Role>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                Type = RoleType.Admin
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Candidate",
                Type = RoleType.Candidate
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Recruiter",
                Type = RoleType.Recruiter
            }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }
}