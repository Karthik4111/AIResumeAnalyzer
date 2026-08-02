using AIResumeAnalyzer.Domain.Enums;
using AIResumeAnalyzer.Domain.Common;


namespace AIResumeAnalyzer.Domain.Entities;

public class Role : BaseEntity
{
    public RoleType Type { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();
}