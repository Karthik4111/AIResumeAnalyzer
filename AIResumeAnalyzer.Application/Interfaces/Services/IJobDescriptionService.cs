using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Application.DTOs.JobDescription;

namespace AIResumeAnalyzer.Application.Interfaces.Services;

public interface IJobDescriptionService
{
    Task<JobDescriptionResponse> CreateAsync(
    CreateJobDescriptionRequest request,
    Guid userId);

    Task<List<JobDescriptionResponse>> GetAllAsync();

    Task<JobDescriptionResponse?> GetByIdAsync(
        Guid id);

    Task<JobDescriptionResponse> UpdateAsync(
        Guid id,
        UpdateJobDescriptionRequest request);

    Task DeleteAsync(Guid id);
}