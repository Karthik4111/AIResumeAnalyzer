using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Application.DTOs.File;

namespace AIResumeAnalyzer.Application.Interfaces.Services;

public interface IFileService
{
    Task<DownloadResumeResponse> DownloadResumeAsync(Guid resumeId);

    Task<List<ResumeVersionResponse>> GetVersionsAsync(Guid resumeId);

    Task<DownloadResumeResponse> DownloadVersionAsync(Guid versionId);

    Task DeleteResumeAsync(Guid resumeId);
}