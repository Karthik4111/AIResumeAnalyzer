using AIResumeAnalyzer.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedOnUtc { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOnUtc { get; set; }

    public string? ModifiedBy { get; set; }

    // NEW
    public bool IsDeleted { get; set; }

    public DateTime? DeletedOnUtc { get; set; }

    public string? DeletedBy { get; set; }
}