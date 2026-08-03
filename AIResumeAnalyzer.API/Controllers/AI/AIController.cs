using AIResumeAnalyzer.Application.DTOs.AI;
using AIResumeAnalyzer.Application.DTOs.Interview;
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

    [HttpPost("interview-questions")]
    public async Task<IActionResult> GenerateInterviewQuestions(
    InterviewQuestionRequest request)
    {
        var result = await _aiService.GenerateInterviewQuestionsAsync(request);

        return Ok(result);
    }

    [HttpGet("interview-questions/{resumeId:guid}")]
    public async Task<IActionResult> GetInterviewQuestions(Guid resumeId)
    {
        var result = await _aiService.GetInterviewQuestionsAsync(resumeId);

        return Ok(result);
    }
}