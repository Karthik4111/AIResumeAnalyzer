using AIResumeAnalyzer.Application.DTOs.Recommendation;
using AIResumeAnalyzer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIResumeAnalyzer.API.Controllers.Recommendation;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;

    public RecommendationController(IRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(CreateRecommendationRequest request)
    {
        var result = await _recommendationService.GenerateAsync(request);

        return Ok(result);
    }

    [HttpGet("{resumeId:guid}")]
    public async Task<IActionResult> Get(Guid resumeId)
    {
        var result = await _recommendationService
            .GetByResumeAsync(resumeId);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _recommendationService.DeleteAsync(id);

        return NoContent();
    }
}