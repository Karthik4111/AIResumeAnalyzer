using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.DTOs.Interview;

public class InterviewQuestionResponse
{
    public List<string> Questions { get; set; } = new();
}