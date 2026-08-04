using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using AIResumeAnalyzer.Application.DTOs.CoverLetter;
using AIResumeAnalyzer.Application.Interfaces.Persistence;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Application.Interfaces.Services;
using AIResumeAnalyzer.Domain.Entities;
using AIResumeAnalyzer.Infrastructure.AI.Models;

namespace AIResumeAnalyzer.Infrastructure.Services.CoverLetter;

public class CoverLetterService : ICoverLetterService
{
    private readonly HttpClient _httpClient;
    private readonly IResumeRepository _resumeRepository;
    private readonly IJobDescriptionRepository _jobDescriptionRepository;
    private readonly IATSReportRepository _atsReportRepository;
    private readonly ICoverLetterRepository _coverLetterRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CoverLetterService(
        HttpClient httpClient,
        IResumeRepository resumeRepository,
        IJobDescriptionRepository jobDescriptionRepository,
        IATSReportRepository atsReportRepository,
        ICoverLetterRepository coverLetterRepository,
        IUnitOfWork unitOfWork)
    {
        _httpClient = httpClient;
        _resumeRepository = resumeRepository;
        _jobDescriptionRepository = jobDescriptionRepository;
        _atsReportRepository = atsReportRepository;
        _coverLetterRepository = coverLetterRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<CoverLetterResponse> GenerateAsync(
    CreateCoverLetterRequest request)
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
You are an expert career coach.

Write a professional cover letter using the resume and job description below.

Rules:
- Maximum 400 words.
- Professional tone.
- Mention relevant skills.
- Mention enthusiasm for the role.
- Return ONLY the cover letter text.

===========================
RESUME
===========================

{latestVersion.ExtractedText}

===========================
JOB DESCRIPTION
===========================

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

        var ollamaResponse =
            await response.Content.ReadFromJsonAsync<OllamaResponse>();

        if (ollamaResponse == null)
            throw new Exception("No response from Ollama.");

        var coverLetter = new Domain.Entities.CoverLetter
        {
            Id = Guid.NewGuid(),
            ATSReportId = atsReport.Id,
            Content = ollamaResponse.Response,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _coverLetterRepository.AddAsync(coverLetter);

        await _unitOfWork.SaveChangesAsync();

        return new CoverLetterResponse
        {
            Id = coverLetter.Id,
            Content = coverLetter.Content,
            CreatedOn = coverLetter.CreatedOnUtc
        };
    }

    public async Task<List<CoverLetterResponse>> GetByResumeAsync(Guid resumeId)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}