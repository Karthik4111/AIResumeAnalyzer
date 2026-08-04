using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Application.DTOs.CoverLetter;

namespace AIResumeAnalyzer.Application.Interfaces.Services;

public interface ICoverLetterService
{
    Task<CoverLetterResponse> GenerateAsync(CreateCoverLetterRequest request);

    Task<List<CoverLetterResponse>> GetByResumeAsync(Guid resumeId);

    Task DeleteAsync(Guid id);
}