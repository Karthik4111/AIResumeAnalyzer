using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Application.DTOs.Pdf;

public class PdfResponse
{
    public byte[] FileBytes { get; set; } = Array.Empty<byte>();

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = "application/pdf";
}