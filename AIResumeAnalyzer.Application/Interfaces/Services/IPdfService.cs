using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Application.DTOs.Pdf;

namespace AIResumeAnalyzer.Application.Interfaces.Services;

public interface IPdfService
{
    Task<PdfResponse> ExportATSReportAsync(Guid atsReportId);

    Task<PdfResponse> ExportCoverLetterAsync(Guid coverLetterId);

    Task<PdfResponse> ExportInterviewQuestionsAsync(Guid atsReportId);
}