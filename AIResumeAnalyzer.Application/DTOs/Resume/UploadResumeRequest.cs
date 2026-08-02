using Microsoft.AspNetCore.Http;

namespace AIResumeAnalyzer.Application.DTOs.Resume;

public class UploadResumeRequest
{
    public IFormFile Resume { get; set; } = default!;
}