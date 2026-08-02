using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Domain.Common;

namespace AIResumeAnalyzer.Domain.Entities;

public class ResumeVersion : BaseAuditableEntity
{
    public Guid ResumeId { get; set; }

    public Resume Resume { get; set; } = null!;

    public int VersionNumber { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string ExtractedText { get; set; } = string.Empty;

    public ICollection<ATSReport> ATSReports { get; set; }= new List<ATSReport>();
}