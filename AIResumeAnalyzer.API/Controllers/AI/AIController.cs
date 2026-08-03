using AIResumeAnalyzer.Application.DTOs.AI;
using AIResumeAnalyzer.Application.Interfaces.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIResumeAnalyzer.API.Controllers.AI;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;

    public AIController(IAIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze(AIAnalysisRequest request)
    {
        var result = await _aiService.AnalyzeResumeAsync(request);

        return Ok(result);
    }
}