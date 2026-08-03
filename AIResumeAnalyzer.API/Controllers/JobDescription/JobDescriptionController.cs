using AIResumeAnalyzer.Application.DTOs.JobDescription;
using AIResumeAnalyzer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AIResumeAnalyzer.API.Controllers.JobDescription;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobDescriptionController : ControllerBase
{
    private readonly IJobDescriptionService _jobDescriptionService;

    public JobDescriptionController(
        IJobDescriptionService jobDescriptionService)
    {
        _jobDescriptionService = jobDescriptionService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateJobDescriptionRequest request)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _jobDescriptionService.CreateAsync(
            request,
            userId);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _jobDescriptionService.GetAllAsync();

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _jobDescriptionService.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateJobDescriptionRequest request)
    {
        var result = await _jobDescriptionService.UpdateAsync(
            id,
            request);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _jobDescriptionService.DeleteAsync(id);

        return NoContent();
    }
}