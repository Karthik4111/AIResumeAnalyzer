using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.DTOs.Interview;

public class InterviewQuestionRequest
{
    public Guid ResumeId { get; set; }

    public Guid JobDescriptionId { get; set; }

    public int NumberOfQuestions { get; set; } = 10;
}