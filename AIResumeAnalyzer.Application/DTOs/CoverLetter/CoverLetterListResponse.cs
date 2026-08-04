using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.DTOs.CoverLetter;

public class CoverLetterListResponse
{
    public List<CoverLetterResponse> CoverLetters { get; set; }= new();
}