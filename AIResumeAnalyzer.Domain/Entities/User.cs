using AIResumeAnalyzer.Domain.Common;

namespace AIResumeAnalyzer.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();

    public ICollection<JobDescription> JobDescriptions { get; set; } = new List<JobDescription>();
}