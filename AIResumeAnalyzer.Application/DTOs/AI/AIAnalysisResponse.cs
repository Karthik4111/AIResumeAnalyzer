using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.DTOs.AI;

public class AIAnalysisResponse
{
    public string OverallFeedback { get; set; } = string.Empty;

    public List<string> Strengths { get; set; } = new();

    public List<string> Improvements { get; set; } = new();

    public List<string> MissingSkills { get; set; } = new();
}
