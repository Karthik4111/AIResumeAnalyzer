using AIResumeAnalyzer.Application.DTOs.JobDescription;
using AIResumeAnalyzer.Application.Interfaces.Persistence;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Application.Interfaces.Services;

namespace AIResumeAnalyzer.Infrastructure.Services.JobDescription;

public class JobDescriptionService : IJobDescriptionService
{
    private readonly IJobDescriptionRepository _jobDescriptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public JobDescriptionService(
        IJobDescriptionRepository jobDescriptionRepository,
        IUnitOfWork unitOfWork)
    {
        _jobDescriptionRepository = jobDescriptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<JobDescriptionResponse> CreateAsync(
        CreateJobDescriptionRequest request,
        Guid userId)
    {
        var jobDescription = new AIResumeAnalyzer.Domain.Entities.JobDescription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title,
            Company = request.Company,
            Description = request.Description,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _jobDescriptionRepository.AddAsync(jobDescription);

        await _unitOfWork.SaveChangesAsync();

        return new JobDescriptionResponse
        {
            Id = jobDescription.Id,
            Title = jobDescription.Title,
            Company = jobDescription.Company,
            Description = jobDescription.Description,
            CreatedOn = jobDescription.CreatedOnUtc
        };
    }

    public async Task<List<JobDescriptionResponse>> GetAllAsync()
    {
        var jobs = await _jobDescriptionRepository.GetAllAsync();

        return jobs.Select(j => new JobDescriptionResponse
        {
            Id = j.Id,
            Title = j.Title,
            Company = j.Company,
            Description = j.Description,
            CreatedOn = j.CreatedOnUtc
        }).ToList();
    }

    public async Task<JobDescriptionResponse?> GetByIdAsync(Guid id)
    {
        var job = await _jobDescriptionRepository.GetByIdAsync(id);

        if (job == null)
            return null;

        return new JobDescriptionResponse
        {
            Id = job.Id,
            Title = job.Title,
            Company = job.Company,
            Description = job.Description,
            CreatedOn = job.CreatedOnUtc
        };
    }

    public async Task<JobDescriptionResponse> UpdateAsync(
        Guid id,
        UpdateJobDescriptionRequest request)
    {
        var job = await _jobDescriptionRepository.GetByIdAsync(id);

        if (job == null)
            throw new Exception("Job Description not found.");

        job.Title = request.Title;
        job.Company = request.Company;
        job.Description = request.Description;
        job.ModifiedOnUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return new JobDescriptionResponse
        {
            Id = job.Id,
            Title = job.Title,
            Company = job.Company,
            Description = job.Description,
            CreatedOn = job.CreatedOnUtc
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var job = await _jobDescriptionRepository.GetByIdAsync(id);

        if (job == null)
            throw new Exception("Job Description not found.");

        job.IsDeleted = true;
        job.DeletedOnUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
    }
}