using AIResumeAnalyzer.Application.DTOs.ATS;
using AIResumeAnalyzer.Application.Interfaces.ATS;
using AIResumeAnalyzer.Application.Interfaces.Persistence;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Infrastructure.Services.ATS;

public class ATSService : IATSService
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IATSReportRepository _atsReportRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ATSService( IResumeRepository resumeRepository, IATSReportRepository atsReportRepository, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _atsReportRepository = atsReportRepository;
        _unitOfWork = unitOfWork;
    }

    private static readonly List<string> KnownSkills = new()
    {
        "C#",
        ".NET",
        ".NET Core",
        ".NET 6",
        ".NET 7",
        ".NET 8",
        "ASP.NET",
        "ASP.NET Core",
        "ASP.NET MVC",
        "Web API",
        "REST API",
        "Entity Framework",
        "Entity Framework Core",
        "SQL",
        "SQL Server",
        "MySQL",
        "Oracle",
        "MongoDB",
        "Azure",
        "Azure DevOps",
        "Docker",
        "Kubernetes",
        "RabbitMQ",
        "Kafka",
        "Redis",
        "React",
        "Angular",
        "JavaScript",
        "TypeScript",
        "HTML",
        "CSS",
        "Git",
        "GitHub",
        "CI/CD",
        "JWT",
        "OAuth",
        "Microservices",
        "LINQ",
        "MVC",
        "WPF",
        "MVVM"
    };

    public async Task<ATSAnalysisResponse> AnalyzeAsync(ATSAnalysisRequest request)
    {
        var resume = await _resumeRepository
            .GetByIdWithVersionsAsync(request.ResumeId);

        if (resume == null)
            throw new Exception("Resume not found.");

        var latestVersion = resume.Versions
            .OrderByDescending(v => v.VersionNumber)
            .FirstOrDefault();

        if (latestVersion == null)
            throw new Exception("Resume version not found.");

        var resumeText = latestVersion.ExtractedText;

        // Extract keywords from Job Description
        var keywords = ExtractKeywords(request.JobDescription);

        // Find matched and missing skills
        var matchedSkills = FindMatchedSkills(
            resumeText,
            keywords);

        var missingSkills = FindMissingSkills(
            keywords,
            matchedSkills);

        // Calculate ATS score
        var score = CalculateATSScore(
            matchedSkills.Count,
            keywords.Count);

        // Save ATS Report
        var report = new AIResumeAnalyzer.Domain.Entities.ATSReport
        {
            Id = Guid.NewGuid(),
            ResumeVersionId = latestVersion.Id,

            // Temporary until Job Description entity is implemented
            JobDescriptionId = Guid.Empty,

            AtsScore = score,

            Summary =
                $"ATS Score: {score}%{Environment.NewLine}" +
                $"Matched Skills: {string.Join(", ", matchedSkills)}{Environment.NewLine}" +
                $"Missing Skills: {string.Join(", ", missingSkills)}",

            CreatedOnUtc = DateTime.UtcNow
        };

        await _atsReportRepository.AddAsync(report);

        await _unitOfWork.SaveChangesAsync();

        // Return response
        return new ATSAnalysisResponse
        {
            ATSScore = score,
            MatchedSkills = matchedSkills,
            MissingSkills = missingSkills
        };
    }

    private List<string> ExtractKeywords(string jobDescription)
    {
        return KnownSkills
            .Where(skill =>
                jobDescription.Contains(
                    skill,
                    StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private List<string> FindMatchedSkills(string resumeText,List<string> keywords)
    {
        return keywords
            .Where(skill =>
                resumeText.Contains(
                    skill,
                    StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private List<string> FindMissingSkills(List<string> keywords,List<string> matchedSkills)
    {
        return keywords
            .Except(
                matchedSkills,
                StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private int CalculateATSScore(int matched,int total)
    {
        if (total == 0)
            return 0;

        return (int)Math.Round(
            (double)matched / total * 100);
    }

    public async Task<List<ATSReport>> GetReportsAsync(Guid resumeId)
    {
        return await _atsReportRepository.GetByResumeIdAsync(resumeId);
    }

    public async Task<ATSReport?> GetLatestReportAsync(Guid resumeId)
    {
        return await _atsReportRepository.GetLatestByResumeIdAsync(resumeId);
    }
}