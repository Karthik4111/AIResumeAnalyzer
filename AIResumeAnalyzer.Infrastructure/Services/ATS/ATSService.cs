using AIResumeAnalyzer.Application.DTOs.ATS;
using AIResumeAnalyzer.Application.Interfaces.ATS;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Domain.Entities;

namespace AIResumeAnalyzer.Infrastructure.Services.ATS;

public class ATSService : IATSService
{
    private readonly IResumeRepository _resumeRepository;

    public ATSService(IResumeRepository resumeRepository)
    {
        _resumeRepository = resumeRepository;
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

        var keywords = ExtractKeywords(request.JobDescription);

        var matchedSkills = FindMatchedSkills(
            resumeText,
            keywords);

        var missingSkills = FindMissingSkills(
            keywords,
            matchedSkills);

        var score = CalculateATSScore(
            matchedSkills.Count,
            keywords.Count);

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

    private List<string> FindMatchedSkills(
        string resumeText,
        List<string> keywords)
    {
        return keywords
            .Where(skill =>
                resumeText.Contains(
                    skill,
                    StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private List<string> FindMissingSkills(
        List<string> keywords,
        List<string> matchedSkills)
    {
        return keywords
            .Except(
                matchedSkills,
                StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private int CalculateATSScore(
        int matched,
        int total)
    {
        if (total == 0)
            return 0;

        return (int)Math.Round(
            (double)matched / total * 100);
    }

    public async Task<List<ATSReport>> GetReportsAsync(Guid resumeId)
    {
        throw new NotImplementedException(
            "ATS report history will be implemented in Chapter 8.");
    }

    public async Task<ATSReport?> GetLatestReportAsync(Guid resumeId)
    {
        throw new NotImplementedException(
            "Latest ATS report retrieval will be implemented in Chapter 8.");
    }

}