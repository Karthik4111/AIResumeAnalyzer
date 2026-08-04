using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Application.DTOs.Email;

namespace AIResumeAnalyzer.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendAsync(SendEmailRequest request);
}
