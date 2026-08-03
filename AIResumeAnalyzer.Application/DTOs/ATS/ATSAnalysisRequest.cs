using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.DTOs.ATS;

public class ATSAnalysisRequest
{
    public Guid ResumeId { get; set; }

    public string JobDescription { get; set; } = string.Empty;
}
