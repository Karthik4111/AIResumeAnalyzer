using AIResumeAnalyzer.Application.DTOs.AI;
using AIResumeAnalyzer.Application.DTOs.Interview;
using AIResumeAnalyzer.Application.Interfaces.AI;
using AIResumeAnalyzer.Application.Interfaces.Persistence;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Domain.Entities;
using AIResumeAnalyzer.Infrastructure.AI.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace AIResumeAnalyzer.Infrastructure.Services.AI;

public class OllamaService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly IResumeRepository _resumeRepository;
    private readonly IJobDescriptionRepository _jobDescriptionRepository;
    private readonly IATSReportRepository _atsReportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInterviewQuestionRepository _interviewQuestionRepository;

    public OllamaService(
    HttpClient httpClient,
    IResumeRepository resumeRepository,
    IJobDescriptionRepository jobDescriptionRepository,
    IATSReportRepository atsReportRepository,
    IInterviewQuestionRepository interviewQuestionRepository,
    IUnitOfWork unitOfWork)
    {
        _httpClient = httpClient;
        _resumeRepository = resumeRepository;
        _jobDescriptionRepository = jobDescriptionRepository;
        _atsReportRepository = atsReportRepository;
        _interviewQuestionRepository = interviewQuestionRepository;
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

    public async Task<InterviewQuestionResponse> GenerateInterviewQuestionsAsync(
    InterviewQuestionRequest request)
    {
        var resume = await _resumeRepository.GetByIdWithVersionsAsync(request.ResumeId);

        if (resume == null)
            throw new Exception("Resume not found.");

        var latestVersion = resume.Versions
            .OrderByDescending(v => v.VersionNumber)
            .FirstOrDefault();

        if (latestVersion == null)
            throw new Exception("Resume version not found.");

        var jobDescription = await _jobDescriptionRepository
            .GetByIdAsync(request.JobDescriptionId);

        if (jobDescription == null)
            throw new Exception("Job Description not found.");

        var prompt = $"""
You are an expert .NET Technical Interviewer.

Generate exactly {request.NumberOfQuestions} interview questions based on the candidate's resume and the job description.

Return ONLY valid JSON.

Schema:

questions : string[]

Rules:
- Return exactly {request.NumberOfQuestions} questions.
- Every question must be a string.
- Do NOT return the array as a string.
- Do NOT return markdown.
- Do NOT use ```json.
- Do NOT add explanations.

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

        var response = await _httpClient.PostAsJsonAsync("api/generate", ollamaRequest);

        response.EnsureSuccessStatusCode();

        var ollamaResponse = await response.Content.ReadFromJsonAsync<OllamaResponse>();

        if (ollamaResponse == null)
            throw new Exception("No response received from Ollama.");

        var result = JsonSerializer.Deserialize<InterviewQuestionResponse>(
            ollamaResponse.Response,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (result == null)
            throw new Exception("Failed to deserialize AI response.");

        result.Questions ??= new List<string>();

        if (result.Questions.Count == 1)
        {
            try
            {
                var parsed = JsonSerializer.Deserialize<List<string>>(result.Questions[0]);

                if (parsed != null && parsed.Count > 0)
                {
                    result.Questions = parsed;
                }
            }
            catch
            {
            }
        }

        var latestReport = await _atsReportRepository
            .GetLatestByResumeIdAsync(request.ResumeId);

        if (latestReport == null)
            throw new Exception("ATS Report not found.");

        foreach (var question in result.Questions)
        {
            await _interviewQuestionRepository.AddAsync(
                new InterviewQuestion
                {
                    Id = Guid.NewGuid(),
                    ATSReportId = latestReport.Id,
                    Question = question,
                    CreatedOnUtc = DateTime.UtcNow
                });
        }

        await _unitOfWork.SaveChangesAsync();

        return result;
    }

    public async Task<InterviewQuestionResponse> GetInterviewQuestionsAsync(Guid resumeId)
    {
        var latestReport = await _atsReportRepository
            .GetLatestByResumeIdAsync(resumeId);

        if (latestReport == null)
            throw new Exception("ATS Report not found.");

        var questions = await _interviewQuestionRepository
            .GetByATSReportIdAsync(latestReport.Id);

        return new InterviewQuestionResponse
        {
            Questions = questions
                .Select(q => q.Question)
                .ToList()
        };
    }
}