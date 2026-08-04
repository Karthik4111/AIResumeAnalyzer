using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Domain.Entities;

namespace AIResumeAnalyzer.Application.Interfaces.Repositories;

public interface ICoverLetterRepository
{
    Task AddAsync(CoverLetter coverLetter);

    Task<List<CoverLetter>> GetByResumeIdAsync(Guid resumeId);

    Task<CoverLetter?> GetByIdAsync(Guid id);
}