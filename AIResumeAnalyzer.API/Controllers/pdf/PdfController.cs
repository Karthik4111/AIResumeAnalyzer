using AIResumeAnalyzer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIResumeAnalyzer.API.Controllers.pdf;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PdfController : ControllerBase
{
    private readonly IPdfService _pdfService;

    public PdfController(IPdfService pdfService)
    {
        _pdfService = pdfService;
    }

    [HttpGet("ats/{atsReportId:guid}")]
    public async Task<IActionResult> ExportATS(Guid atsReportId)
    {
        var pdf = await _pdfService.ExportATSReportAsync(atsReportId);

        return File(
            pdf.FileBytes,
            pdf.ContentType,
            pdf.FileName);
    }

    [HttpGet("cover-letter/{coverLetterId:guid}")]
    public async Task<IActionResult> ExportCoverLetter(Guid coverLetterId)
    {
        var pdf =
            await _pdfService.ExportCoverLetterAsync(coverLetterId);

        return File(
            pdf.FileBytes,
            pdf.ContentType,
            pdf.FileName);
    }

    [HttpGet("interview/{atsReportId:guid}")]
    public async Task<IActionResult> ExportInterview(Guid atsReportId)
    {
        var pdf =
            await _pdfService.ExportInterviewQuestionsAsync(atsReportId);

        return File(
            pdf.FileBytes,
            pdf.ContentType,
            pdf.FileName);
    }
}