using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIResumeAnalyzer.Infrastructure.AI.Models;

public class OllamaRequest
{
    public string Model { get; set; } = "llama3.2";

    public string Prompt { get; set; } = string.Empty;

    public bool Stream { get; set; } = false;
}