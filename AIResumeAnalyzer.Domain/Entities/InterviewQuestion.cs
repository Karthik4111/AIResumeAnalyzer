using AIResumeAnalyzer.Domain.Common;

namespace AIResumeAnalyzer.Domain.Entities;

public class InterviewQuestion : BaseAuditableEntity
{
    public Guid ATSReportId { get; set; }

    public ATSReport ATSReport { get; set; } = null!;

    public string Question { get; set; } = string.Empty;
}