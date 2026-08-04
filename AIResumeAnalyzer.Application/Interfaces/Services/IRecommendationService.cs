using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Application.DTOs.Recommendation;

namespace AIResumeAnalyzer.Application.Interfaces.Services;

public interface IRecommendationService
{
    Task<RecommendationResponse> GenerateAsync(CreateRecommendationRequest request);

    Task<List<RecommendationResponse>> GetByResumeAsync(Guid resumeId);

    Task DeleteAsync(Guid id);
}