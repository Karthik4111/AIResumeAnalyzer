using System.Net.Http.Json;
using System.Text.Json;
using AIResumeAnalyzer.Application.DTOs.AI;
using AIResumeAnalyzer.Application.Interfaces.AI;
using AIResumeAnalyzer.Application.Interfaces.Persistence;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Domain.Entities;
using AIResumeAnalyzer.Infrastructure.AI.Models;

namespace AIResumeAnalyzer.Infrastructure.Services.AI;

public class OllamaService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly IResumeRepository _resumeRepository;
    private readonly IJobDescriptionRepository _jobDescriptionRepository;
    private readonly IATSReportRepository _atsReportRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OllamaService(
        HttpClient httpClient,
        IResumeRepository resumeRepository,
        IJobDescriptionRepository jobDescriptionRepository,
        IATSReportRepository atsReportRepository,
        IUnitOfWork unitOfWork)
    {
        _httpClient = httpClient;
        _resumeRepository = resumeRepository;
        _jobDescriptionRepository = jobDescriptionRepository;
        _atsReportRepository = atsReportRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AIAnalysisResponse> AnalyzeResumeAsync(
        AIAnalysisRequest request)
    {
        // Load Resume
        var resume = await _resumeRepository
            .GetByIdWithVersionsAsync(request.ResumeId);

        if (resume == null)
            throw new Exception("Resume not found.");

        var latestVersion = resume.Versions
            .OrderByDescending(v => v.VersionNumber)
            .FirstOrDefault();

        if (latestVersion == null)
            throw new Exception("Resume version not found.");

        // Load Job Description
        var jobDescription = await _jobDescriptionRepository
            .GetByIdAsync(request.JobDescriptionId);

        if (jobDescription == null)
            throw new Exception("Job Description not found.");

        // Build Prompt
        var prompt = $"""
You are an expert ATS Resume Reviewer.

Analyze the following resume against the given job description.

Return ONLY valid JSON.

The JSON MUST follow this schema exactly.

overallFeedback : string
strengths : string[]
improvements : string[]
missingSkills : string[]

Rules:
- overallFeedback must never be empty.
- strengths must contain at least 3 items.
- improvements must contain at least 3 items.
- missingSkills must contain all important missing technical skills.
- Return ONLY JSON.
- Do not return markdown.
- Do not use ```json.
- Do not add explanations outside the JSON.

==================================================
RESUME
==================================================

{latestVersion.ExtractedText}

==================================================
JOB DESCRIPTION
==================================================

{jobDescription.Description}
""";

        var ollamaRequest = new OllamaRequest
        {
            Model = "llama3.2",
            Prompt = prompt,
            Stream = false
        };

        var response = await _httpClient.PostAsJsonAsync(
            "api/generate",
            ollamaRequest);

        response.EnsureSuccessStatusCode();

        var ollamaResponse = await response.Content
            .ReadFromJsonAsync<OllamaResponse>();

        if (ollamaResponse == null)
            throw new Exception("No response received from Ollama.");

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<AIAnalysisResponse>(
                ollamaResponse.Response,
                options);

            if (result == null)
                throw new Exception("Failed to deserialize AI response.");

            result.OverallFeedback ??= string.Empty;
            result.Strengths ??= new List<string>();
            result.Improvements ??= new List<string>();
            result.MissingSkills ??= new List<string>();

            var report = new ATSReport
            {
                Id = Guid.NewGuid(),
                ResumeVersionId = latestVersion.Id,
                JobDescriptionId = jobDescription.Id,
                AtsScore = 0,
                Summary = result.OverallFeedback,
                CreatedOnUtc = DateTime.UtcNow
            };

            await _atsReportRepository.AddAsync(report);
            await _unitOfWork.SaveChangesAsync();

            return result;
        }
        catch
        {
            return new AIAnalysisResponse
            {
                OverallFeedback = ollamaResponse.Response,
                Strengths = new List<string>(),
                Improvements = new List<string>(),
                MissingSkills = new List<string>()
            };
        }
    }
}