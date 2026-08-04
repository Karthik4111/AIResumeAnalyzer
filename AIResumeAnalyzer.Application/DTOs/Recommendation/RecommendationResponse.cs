using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.DTOs.Recommendation;

public class RecommendationResponse
{
    public Guid Id { get; set; }

    public List<string> Recommendations { get; set; } = new();

    public DateTime CreatedOn { get; set; }
}