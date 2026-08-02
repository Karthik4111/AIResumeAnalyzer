namespace AIResumeAnalyzer.Application.DTOs.Resume;

public class ResumeResponse
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int Version { get; set; }

    public DateTime UploadedOn { get; set; }
}