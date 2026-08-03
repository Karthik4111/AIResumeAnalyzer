using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Domain.Entities;

namespace AIResumeAnalyzer.Application.Interfaces.Repositories;

public interface IInterviewQuestionRepository
{
    Task AddAsync(InterviewQuestion interviewQuestion);

    Task<List<InterviewQuestion>> GetByATSReportIdAsync(Guid atsReportId);
}