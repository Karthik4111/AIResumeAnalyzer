using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.DTOs.Admin;

public class AdminDashboardResponse
{
    public int TotalUsers { get; set; }

    public int TotalResumes { get; set; }

    public int TotalJobDescriptions { get; set; }

    public int TotalATSReports { get; set; }

    public int TotalCoverLetters { get; set; }

    public int TotalInterviewQuestions { get; set; }

    public int TotalRecommendations { get; set; }
}