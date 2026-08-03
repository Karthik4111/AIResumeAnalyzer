using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Infrastructure.AI.Models;

public class OllamaResponse
{
    public string Model { get; set; } = string.Empty;

    public string Response { get; set; } = string.Empty;

    public bool Done { get; set; }
}