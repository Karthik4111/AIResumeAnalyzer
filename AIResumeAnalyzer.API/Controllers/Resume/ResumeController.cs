using AIResumeAnalyzer.Application.DTOs.Resume;
using AIResumeAnalyzer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace AIResumeAnalyzer.API.Controllers.Resume;



[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting("DefaultPolicy")]
public class ResumeController : ControllerBase
{
    private readonly IResumeService _resumeService;

    public ResumeController(IResumeService resumeService)
    {
        _resumeService = resumeService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadResumeRequest request)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _resumeService.UploadAsync(request, userId);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyResumes()
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var resumes = await _resumeService.GetMyResumesAsync(userId);

        return Ok(resumes);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var resume = await _resumeService.GetByIdAsync(id);

        if (resume == null)
            return NotFound();

        return Ok(resume);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _resumeService.DeleteAsync(id);

        return NoContent();
    }

    [HttpGet("{resumeId:guid}/download")]
    public async Task<IActionResult> Download(Guid resumeId)
    {
        var result = await _resumeService.DownloadAsync(resumeId);

        return File(
            result.FileBytes,
            result.ContentType,
            result.FileName);
    }

    [HttpPost("{resumeId:guid}/versions")]
    public async Task<IActionResult> UploadVersion(
    Guid resumeId,
    [FromForm] UploadResumeVersionRequest request)
    {
        var result = await _resumeService.UploadVersionAsync(
            resumeId,
            request);

        return Ok(result);
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var userId = Guid.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var result =
            await _resumeService.GetDashboardAsync(userId);

        return Ok(result);
    }
}