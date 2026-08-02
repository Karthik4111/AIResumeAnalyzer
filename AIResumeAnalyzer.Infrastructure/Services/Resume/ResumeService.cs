using AIResumeAnalyzer.Application.DTOs.Resume;
using AIResumeAnalyzer.Application.Interfaces.Persistence;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Application.Interfaces.Services;
using AIResumeAnalyzer.Domain.Enums;

namespace AIResumeAnalyzer.Infrastructure.Services.Resume;

public class ResumeService : IResumeService
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IResumeVersionRepository _resumeVersionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ResumeService(
        IResumeRepository resumeRepository,
        IResumeVersionRepository resumeVersionRepository,
        IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _resumeVersionRepository = resumeVersionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResumeResponse> UploadAsync(
        UploadResumeRequest request,
        Guid userId)
    {
        // Validate file
        if (request.Resume == null || request.Resume.Length == 0)
            throw new Exception("Please select a resume.");

        // Validate extension
        var extension = Path.GetExtension(request.Resume.FileName).ToLower();

        if (extension != ".pdf" && extension != ".docx")
            throw new Exception("Only PDF and DOCX files are allowed.");

        // Validate file size (5 MB)
        const long maxFileSize = 5 * 1024 * 1024;

        if (request.Resume.Length > maxFileSize)
            throw new Exception("Maximum file size is 5 MB.");

        // Create Upload folder
        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads",
            "Resumes");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Generate unique filename
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(
            uploadsFolder,
            uniqueFileName);

        // Save physical file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.Resume.CopyToAsync(stream);
        }

        // Create Resume
        var resume = new AIResumeAnalyzer.Domain.Entities.Resume
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = Path.GetFileNameWithoutExtension(request.Resume.FileName),
            Status = ResumeStatus.Active,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _resumeRepository.AddAsync(resume);

        // Create Resume Version
        var resumeVersion = new AIResumeAnalyzer.Domain.Entities.ResumeVersion
        {
            Id = Guid.NewGuid(),
            ResumeId = resume.Id,
            VersionNumber = 1,
            FileName = request.Resume.FileName,
            FilePath = filePath,
            ExtractedText = string.Empty,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _resumeVersionRepository.AddAsync(resumeVersion);

        // Save transaction
        await _unitOfWork.SaveChangesAsync();

        // Return response
        return new ResumeResponse
        {
            Id = resume.Id,
            FileName = resumeVersion.FileName,
            Status = resume.Status.ToString(),
            Version = resumeVersion.VersionNumber,
            UploadedOn = resumeVersion.CreatedOnUtc
        };
    }

    public async Task<List<ResumeResponse>> GetMyResumesAsync(Guid userId)
    {
        var resumes = await _resumeRepository.GetByUserIdAsync(userId);

        return resumes.Select(r =>
        {
            var latestVersion = r.Versions
                .OrderByDescending(v => v.VersionNumber)
                .FirstOrDefault();

            return new ResumeResponse
            {
                Id = r.Id,
                FileName = latestVersion?.FileName ?? string.Empty,
                Status = r.Status.ToString(),
                Version = latestVersion?.VersionNumber ?? 0,
                UploadedOn = latestVersion?.CreatedOnUtc ?? r.CreatedOnUtc
            };
        }).ToList();
    }

    public async Task<ResumeResponse?> GetByIdAsync(Guid resumeId)
    {
        var resume = await _resumeRepository.GetByIdWithVersionsAsync(resumeId);

        if (resume == null)
            return null;

        var latestVersion = resume.Versions
            .OrderByDescending(v => v.VersionNumber)
            .FirstOrDefault();

        return new ResumeResponse
        {
            Id = resume.Id,
            FileName = latestVersion?.FileName ?? string.Empty,
            Status = resume.Status.ToString(),
            Version = latestVersion?.VersionNumber ?? 0,
            UploadedOn = latestVersion?.CreatedOnUtc ?? resume.CreatedOnUtc
        };
    }

    public async Task DeleteAsync(Guid resumeId)
    {
        var resume = await _resumeRepository.GetByIdAsync(resumeId);

        if (resume == null)
            throw new Exception("Resume not found.");

        resume.IsDeleted = true;
        resume.DeletedOnUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
    }
}