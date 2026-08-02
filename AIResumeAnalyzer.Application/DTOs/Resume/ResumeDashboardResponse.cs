using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.DTOs.Resume;

public class ResumeDashboardResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int LatestVersion { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime UploadedOn { get; set; }
}
