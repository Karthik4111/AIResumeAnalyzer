using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Domain.Common;

namespace AIResumeAnalyzer.Domain.Entities;

public class CoverLetter : BaseAuditableEntity
{
    public Guid ATSReportId { get; set; }

    public ATSReport ATSReport { get; set; } = null!;

    public string Content { get; set; } = string.Empty;
}
