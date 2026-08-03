using AIResumeAnalyzer.Application.DTOs.ATS;
using AIResumeAnalyzer.Application.Interfaces.ATS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIResumeAnalyzer.API.Controllers.ATS;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ATSController : ControllerBase
{
    private readonly IATSService _atsService;

    public ATSController(IATSService atsService)
    {
        _atsService = atsService;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze(ATSAnalysisRequest request)
    {
        var result = await _atsService.AnalyzeAsync(request);

        return Ok(result);
    }

    [HttpGet("{resumeId}")]
    public async Task<IActionResult> GetReports(Guid resumeId)
    {
        var reports = await _atsService.GetReportsAsync(resumeId);

        return Ok(reports);
    }

    [HttpGet("latest/{resumeId}")]
    public async Task<IActionResult> GetLatest(Guid resumeId)
    {
        var report = await _atsService.GetLatestReportAsync(resumeId);

        if (report == null)
            return NotFound();

        return Ok(report);
    }
}