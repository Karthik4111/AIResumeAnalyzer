using AIResumeAnalyzer.Domain.Common;

namespace AIResumeAnalyzer.Domain.Entities;

public class ATSReport : BaseAuditableEntity
{
    public Guid ResumeVersionId { get; set; }

    public ResumeVersion ResumeVersion { get; set; } = null!;

    public Guid JobDescriptionId { get; set; }

    public JobDescription JobDescription { get; set; } = null!;

    public decimal AtsScore { get; set; }

    public string Summary { get; set; } = string.Empty;

    public ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();

    public ICollection<InterviewQuestion> InterviewQuestions { get; set; } = new List<InterviewQuestion>();

    public CoverLetter? CoverLetter { get; set; }
}