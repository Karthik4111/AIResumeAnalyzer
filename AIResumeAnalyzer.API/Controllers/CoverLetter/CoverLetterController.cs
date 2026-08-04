using AIResumeAnalyzer.Application.DTOs.CoverLetter;
using AIResumeAnalyzer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIResumeAnalyzer.API.Controllers.CoverLetter;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoverLetterController : ControllerBase
{
    private readonly ICoverLetterService _coverLetterService;

    public CoverLetterController(ICoverLetterService coverLetterService)
    {
        _coverLetterService = coverLetterService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        CreateCoverLetterRequest request)
    {
        var result = await _coverLetterService.GenerateAsync(request);

        return Ok(result);
    }

    [HttpGet("{resumeId:guid}")]
    public async Task<IActionResult> Get(Guid resumeId)
    {
        var result = await _coverLetterService
            .GetByResumeAsync(resumeId);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _coverLetterService.DeleteAsync(id);

        return NoContent();
    }
}