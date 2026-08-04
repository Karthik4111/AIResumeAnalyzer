using AIResumeAnalyzer.Application.DTOs.Email;
using AIResumeAnalyzer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIResumeAnalyzer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(
        SendEmailRequest request)
    {
        await _emailService.SendAsync(request);

        return Ok(new
        {
            Message = "Email sent successfully."
        });
    }
}