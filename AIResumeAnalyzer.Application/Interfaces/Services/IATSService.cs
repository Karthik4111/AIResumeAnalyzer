using AIResumeAnalyzer.Application.DTOs.ATS;
using AIResumeAnalyzer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.Interfaces.ATS;

public interface IATSService
{
    Task<ATSAnalysisResponse> AnalyzeAsync(ATSAnalysisRequest request);

    Task<List<ATSReport>> GetReportsAsync(Guid resumeId);

    Task<ATSReport?> GetLatestReportAsync(Guid resumeId);
}