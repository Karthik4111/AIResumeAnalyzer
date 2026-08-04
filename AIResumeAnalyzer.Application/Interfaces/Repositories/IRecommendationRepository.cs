using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Domain.Entities;

namespace AIResumeAnalyzer.Application.Interfaces.Repositories;

public interface IRecommendationRepository
{
    Task AddAsync(Recommendation recommendation);

    Task<List<Recommendation>> GetByResumeIdAsync(Guid resumeId);

    Task<Recommendation?> GetByIdAsync(Guid id);
}