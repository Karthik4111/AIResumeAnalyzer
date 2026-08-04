using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Domain.Entities;

namespace AIResumeAnalyzer.Application.Interfaces.Repositories;

public interface IFileRepository
{
    Task<Resume?> GetResumeWithVersionsAsync(Guid resumeId);

    Task<ResumeVersion?> GetVersionAsync(Guid versionId);

    Task DeleteResumeAsync(Guid resumeId);
}