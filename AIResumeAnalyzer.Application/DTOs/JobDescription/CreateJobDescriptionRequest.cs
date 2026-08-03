using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.DTOs.JobDescription;

public class CreateJobDescriptionRequest
{
    public string Title { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
