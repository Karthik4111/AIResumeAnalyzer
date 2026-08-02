using AIResumeAnalyzer.Domain.Common;

namespace AIResumeAnalyzer.Domain.Entities;

public class Recommendation : BaseAuditableEntity
{
    public Guid ATSReportId { get; set; }

    public ATSReport ATSReport { get; set; } = null!;

    public string Content { get; set; } = string.Empty;
}