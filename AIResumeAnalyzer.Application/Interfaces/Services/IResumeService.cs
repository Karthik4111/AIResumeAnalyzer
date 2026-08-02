using AIResumeAnalyzer.Application.DTOs.Resume;

namespace AIResumeAnalyzer.Application.Interfaces.Services;

public interface IResumeService
{
    Task<ResumeResponse> UploadAsync(
        UploadResumeRequest request,
        Guid userId);

    Task<List<ResumeResponse>> GetMyResumesAsync(Guid userId);

    Task<ResumeResponse?> GetByIdAsync(Guid resumeId);

    Task DeleteAsync(Guid resumeId);
}