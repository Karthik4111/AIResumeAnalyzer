using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Application.DTOs.File;
using AIResumeAnalyzer.Application.Interfaces.Persistence;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Application.Interfaces.Services;

namespace AIResumeAnalyzer.Infrastructure.Services.File;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FileService(IFileRepository fileRepository,IUnitOfWork unitOfWork)
    {
        _fileRepository = fileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ResumeVersionResponse>> GetVersionsAsync(Guid resumeId)
    {
        var resume = await _fileRepository.GetResumeWithVersionsAsync(resumeId);

        if (resume == null)
            throw new Exception("Resume not found.");

        return resume.Versions
            .OrderByDescending(x => x.VersionNumber)
            .Select(x => new ResumeVersionResponse
            {
                Id = x.Id,
                VersionNumber = x.VersionNumber,
                FileName = x.FileName,
                CreatedOn = x.CreatedOnUtc
            })
            .ToList();
    }

    public async Task DeleteResumeAsync(Guid resumeId)
    {
        await _fileRepository.DeleteResumeAsync(resumeId);

        await _unitOfWork.SaveChangesAsync();
    }

    public Task<DownloadResumeResponse> DownloadResumeAsync(Guid resumeId)
    {
        throw new NotImplementedException();
    }

    public Task<DownloadResumeResponse> DownloadVersionAsync(Guid versionId)
    {
        throw new NotImplementedException();
    }
}