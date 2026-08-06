using AIResumeAnalyzer.Application.DTOs.Resume;
using AIResumeAnalyzer.Application.Interfaces.Persistence;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Application.Interfaces.ResumeParsing;
using AIResumeAnalyzer.Application.Interfaces.Services;
using AIResumeAnalyzer.Domain.Enums;
using AIResumeAnalyzer.Application.Interfaces.Services;

namespace AIResumeAnalyzer.Infrastructure.Services.Resume;

public class ResumeService : IResumeService
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IResumeVersionRepository _resumeVersionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IResumeParserService _resumeParserService;
    private readonly ICacheService _cacheService;

    public ResumeService(
    IResumeRepository resumeRepository,
    IResumeVersionRepository resumeVersionRepository,
    IResumeParserService resumeParserService,
    IUnitOfWork unitOfWork,
    ICacheService cacheService)
    {
        _resumeRepository = resumeRepository;
        _resumeVersionRepository = resumeVersionRepository;
        _resumeParserService = resumeParserService;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<ResumeResponse> UploadAsync(
        UploadResumeRequest request,
        Guid userId)
    {
        // Validate file
        if (request.Resume == null || request.Resume.Length == 0)
            throw new Exception("Please select a resume.");

        var extension = Path.GetExtension(request.Resume.FileName).ToLower();

        if (extension != ".pdf" && extension != ".docx")
            throw new Exception("Only PDF and DOCX files are allowed.");

        const long maxFileSize = 5 * 1024 * 1024;

        if (request.Resume.Length > maxFileSize)
            throw new Exception("Maximum file size is 5 MB.");

        // Create upload folder
        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads",
            "Resumes");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Save physical file
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.Resume.CopyToAsync(stream);
        }

        // Extract text
        var extractedText =
            await _resumeParserService.ExtractTextAsync(filePath);

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
            ExtractedText = extractedText,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _resumeVersionRepository.AddAsync(resumeVersion);

        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync($"dashboard_{userId}");
        await _cacheService.RemoveAsync($"resume_{resume.Id}");

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
        var cacheKey = $"resume_{resumeId}";

        var cachedResume =
            await _cacheService.GetAsync<ResumeResponse>(cacheKey);

        if (cachedResume != null)
        {
            return cachedResume;
        }

        var resume = await _resumeRepository.GetByIdWithVersionsAsync(resumeId);

        if (resume == null)
            return null;

        var latestVersion = resume.Versions
            .OrderByDescending(v => v.VersionNumber)
            .FirstOrDefault();

        var response = new ResumeResponse
        {
            Id = resume.Id,
            FileName = latestVersion?.FileName ?? string.Empty,
            Status = resume.Status.ToString(),
            Version = latestVersion?.VersionNumber ?? 0,
            UploadedOn = latestVersion?.CreatedOnUtc ?? resume.CreatedOnUtc
        };

        await _cacheService.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(10));

        return response;
    }

    public async Task DeleteAsync(Guid resumeId)
    {
        var resume = await _resumeRepository.GetByIdAsync(resumeId);

        if (resume == null)
            throw new Exception("Resume not found.");

        resume.IsDeleted = true;
        resume.DeletedOnUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();


        await _cacheService.RemoveAsync($"dashboard_{resume.UserId}");
        await _cacheService.RemoveAsync($"resume_{resumeId}");
    }

    public async Task<(byte[] FileBytes, string FileName, string ContentType)> DownloadAsync(Guid resumeId)
    {
        var version = await _resumeVersionRepository.GetLatestVersionAsync(resumeId);

        if (version == null)
            throw new Exception("Resume not found.");

        if (string.IsNullOrWhiteSpace(version.FilePath))
            throw new Exception("Resume file path is missing.");

        if (!System.IO.File.Exists(version.FilePath))
            throw new Exception("Resume file not found.");

        var bytes = await System.IO.File.ReadAllBytesAsync(version.FilePath);

        var extension = Path.GetExtension(version.FileName).ToLowerInvariant();

        var contentType = extension switch
        {
            ".pdf" => "application/pdf",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };

        return (bytes, version.FileName, contentType);
    }

    public async Task<ResumeResponse> UploadVersionAsync(Guid resumeId,UploadResumeVersionRequest request)
    {
        var resume = await _resumeRepository.GetByIdAsync(resumeId);

        if (resume == null)
            throw new Exception("Resume not found.");

        if (request.Resume == null || request.Resume.Length == 0)
            throw new Exception("Please select a resume.");

        var extension = Path.GetExtension(request.Resume.FileName).ToLower();

        if (extension != ".pdf" && extension != ".docx")
            throw new Exception("Only PDF and DOCX files are allowed.");

        const long maxFileSize = 5 * 1024 * 1024;

        if (request.Resume.Length > maxFileSize)
            throw new Exception("Maximum file size is 5 MB.");

        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads",
            "Resumes");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.Resume.CopyToAsync(stream);
        }

        // Extract text
        var extractedText =
            await _resumeParserService.ExtractTextAsync(filePath);

        var latestVersion =
            await _resumeVersionRepository.GetLatestVersionNumberAsync(resumeId);

        latestVersion++;

        var resumeVersion = new AIResumeAnalyzer.Domain.Entities.ResumeVersion
        {
            Id = Guid.NewGuid(),
            ResumeId = resumeId,
            VersionNumber = latestVersion,
            FileName = request.Resume.FileName,
            FilePath = filePath,
            ExtractedText = extractedText,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _resumeVersionRepository.AddAsync(resumeVersion);

        await _unitOfWork.SaveChangesAsync();

        // Clear dashboard cache
        await _cacheService.RemoveAsync($"dashboard_{resume.UserId}");

        await _cacheService.RemoveAsync($"resume_{resumeId}");

        return new ResumeResponse
        {
            Id = resumeId,
            FileName = resumeVersion.FileName,
            Status = resume.Status.ToString(),
            Version = resumeVersion.VersionNumber,
            UploadedOn = resumeVersion.CreatedOnUtc
        };
    }

    public async Task<List<ResumeDashboardResponse>> GetDashboardAsync(Guid userId)
    {
        var cacheKey = $"dashboard_{userId}";

        var cachedData =
            await _cacheService.GetAsync<List<ResumeDashboardResponse>>(cacheKey);

        if (cachedData != null)
        {
            return cachedData;
        }

        var resumes = await _resumeRepository.GetByUserIdAsync(userId);

        var dashboard = resumes.Select(r =>
        {
            var latestVersion = r.Versions
                .OrderByDescending(v => v.VersionNumber)
                .FirstOrDefault();

            return new ResumeDashboardResponse
            {
                Id = r.Id,
                Title = r.Title,
                LatestVersion = latestVersion?.VersionNumber ?? 0,
                Status = r.Status.ToString(),
                UploadedOn = latestVersion?.CreatedOnUtc ?? r.CreatedOnUtc
            };
        }).ToList();

        await _cacheService.SetAsync(
            cacheKey,
            dashboard,
            TimeSpan.FromMinutes(5));

        return dashboard;
    }
}