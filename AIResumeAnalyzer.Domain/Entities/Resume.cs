using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Domain.Common;
using AIResumeAnalyzer.Domain.Enums;

namespace AIResumeAnalyzer.Domain.Entities;

public class Resume : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;

    public ResumeStatus Status { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<ResumeVersion> Versions { get; set; } = new List<ResumeVersion>();
}
