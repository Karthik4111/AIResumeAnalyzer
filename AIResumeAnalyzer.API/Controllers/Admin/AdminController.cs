using AIResumeAnalyzer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIResumeAnalyzer.API.Controllers.Admin;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var result = await _adminService.GetDashboardAsync();

        return Ok(result);
    }

    [HttpGet("users")]
    public async Task<IActionResult> Users()
    {
        var result = await _adminService.GetUsersAsync();

        return Ok(result);
    }

    [HttpDelete("users/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _adminService.DeleteUserAsync(id);

        return NoContent();
    }
}