using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http.Json;
using AIResumeAnalyzer.Application.DTOs.Recommendation;
using AIResumeAnalyzer.Application.Interfaces.Persistence;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Application.Interfaces.Services;
using AIResumeAnalyzer.Domain.Entities;
using AIResumeAnalyzer.Infrastructure.AI.Models;

namespace AIResumeAnalyzer.Infrastructure.Services.Recommendation;

public class RecommendationService : IRecommendationService
{
    private readonly HttpClient _httpClient;
    private readonly IResumeRepository _resumeRepository;
    private readonly IJobDescriptionRepository _jobDescriptionRepository;
    private readonly IATSReportRepository _atsReportRepository;
    private readonly IRecommendationRepository _recommendationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecommendationService(
        HttpClient httpClient,
        IResumeRepository resumeRepository,
        IJobDescriptionRepository jobDescriptionRepository,
        IATSReportRepository atsReportRepository,
        IRecommendationRepository recommendationRepository,
        IUnitOfWork unitOfWork)
    {
        _httpClient = httpClient;
        _resumeRepository = resumeRepository;
        _jobDescriptionRepository = jobDescriptionRepository;
        _atsReportRepository = atsReportRepository;
        _recommendationRepository = recommendationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RecommendationResponse> GenerateAsync(
    CreateRecommendationRequest request)
    {
        var resume = await _resumeRepository
            .GetByIdWithVersionsAsync(request.ResumeId);

        if (resume == null)
            throw new Exception("Resume not found.");

        var latestVersion = resume.Versions
            .OrderByDescending(x => x.VersionNumber)
            .First();

        var jobDescription = await _jobDescriptionRepository
            .GetByIdAsync(request.JobDescriptionId);

        if (jobDescription == null)
            throw new Exception("Job Description not found.");

        var atsReport = await _atsReportRepository
            .GetLatestByResumeIdAsync(request.ResumeId);

        if (atsReport == null)
            throw new Exception("ATS Report not found.");

        var prompt = $"""
You are an expert ATS Resume Reviewer.

Analyze the resume against the job description.

Generate exactly 8 actionable recommendations.

Return ONLY plain text.

Each recommendation should be on a new line.

=========================
RESUME
=========================

{latestVersion.ExtractedText}

=========================
JOB DESCRIPTION
=========================

{jobDescription.Description}
""";

        var response = await _httpClient.PostAsJsonAsync(
            "api/generate",
            new OllamaRequest
            {
                Model = "llama3.2",
                Prompt = prompt,
                Stream = false
            });

        response.EnsureSuccessStatusCode();

        var ollamaResponse =
            await response.Content.ReadFromJsonAsync<OllamaResponse>();

        if (ollamaResponse == null)
            throw new Exception("No AI response.");

        var recommendations = ollamaResponse.Response
            .Split(Environment.NewLine,
                StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        foreach (var recommendation in recommendations)
        {
            await _recommendationRepository.AddAsync(
                new Domain.Entities.Recommendation
                {
                    Id = Guid.NewGuid(),
                    ATSReportId = atsReport.Id,
                    Content = recommendation,
                    CreatedOnUtc = DateTime.UtcNow
                });
        }

        await _unitOfWork.SaveChangesAsync();

        return new RecommendationResponse
        {
            Id = Guid.NewGuid(),
            Recommendations = recommendations,
            CreatedOn = DateTime.UtcNow
        };
    }

    public async Task<List<RecommendationResponse>> GetByResumeAsync(Guid resumeId)
    {
        var recommendations = await _recommendationRepository
            .GetByResumeIdAsync(resumeId);

        return recommendations
            .GroupBy(x => x.CreatedOnUtc)
            .Select(g => new RecommendationResponse
            {
                Id = g.First().Id,
                Recommendations = g
                    .Select(x => x.Content)
                    .ToList(),
                CreatedOn = g.Key
            })
            .OrderByDescending(x => x.CreatedOn)
            .ToList();
    }

    public async Task DeleteAsync(Guid id)
    {
        var recommendation = await _recommendationRepository
            .GetByIdAsync(id);

        if (recommendation == null)
            throw new Exception("Recommendation not found.");

        recommendation.IsDeleted = true;
        recommendation.DeletedOnUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
    }

}