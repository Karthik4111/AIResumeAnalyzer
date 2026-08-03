using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIResumeAnalyzer.Application.DTOs.AI;

namespace AIResumeAnalyzer.Application.Interfaces.AI;

public interface IAIService
{
    Task<AIAnalysisResponse> AnalyzeResumeAsync(AIAnalysisRequest request);
}