using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.DTOs.ATS;

public class ATSAnalysisResponse
{
    public int ATSScore { get; set; }

    public List<string> MatchedSkills { get; set; } = new();

    public List<string> MissingSkills { get; set; } = new();
}
