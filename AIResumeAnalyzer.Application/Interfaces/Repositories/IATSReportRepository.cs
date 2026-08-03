using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Domain.Entities;

namespace AIResumeAnalyzer.Application.Interfaces.Repositories;

public interface IATSReportRepository : IGenericRepository<ATSReport>
{
    Task<List<ATSReport>> GetByResumeIdAsync(Guid resumeId);

    Task<ATSReport?> GetLatestByResumeIdAsync(Guid resumeId);
}