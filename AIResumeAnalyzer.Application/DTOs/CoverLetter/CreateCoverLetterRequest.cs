using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.DTOs.CoverLetter;

public class CreateCoverLetterRequest
{
    public Guid ResumeId { get; set; }

    public Guid JobDescriptionId { get; set; }
}