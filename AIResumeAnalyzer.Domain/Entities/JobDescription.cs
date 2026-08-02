using AIResumeAnalyzer.Domain.Common;

namespace AIResumeAnalyzer.Domain.Entities;

public class JobDescription : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<ATSReport> ATSReports { get; set; } = new List<ATSReport>();
}