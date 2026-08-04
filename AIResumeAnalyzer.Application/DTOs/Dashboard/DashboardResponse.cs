using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.DTOs.Dashboard;

public class DashboardResponse
{
    public int TotalResumes { get; set; }

    public int TotalJobDescriptions { get; set; }

    public int TotalATSReports { get; set; }

    public int TotalCoverLetters { get; set; }

    public int TotalInterviewQuestions { get; set; }

    public int TotalRecommendations { get; set; }

    public double AverageATSScore { get; set; }

    public int HighestATSScore { get; set; }
}