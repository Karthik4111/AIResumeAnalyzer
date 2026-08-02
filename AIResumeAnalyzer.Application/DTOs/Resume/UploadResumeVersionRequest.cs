using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace AIResumeAnalyzer.Application.DTOs.Resume;

public class UploadResumeVersionRequest
{
    public IFormFile Resume { get; set; } = default!;
}