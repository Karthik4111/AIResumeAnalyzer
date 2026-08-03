using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIResumeAnalyzer.Application.Interfaces.ResumeParsing;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace AIResumeAnalyzer.Infrastructure.Services.ResumeParsing;

public class ResumeParserService : IResumeParserService
{
    public async Task<string> ExtractTextAsync(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();

        return extension switch
        {
            ".pdf" => await ExtractPdfTextAsync(filePath),
            ".docx" => await ExtractDocxTextAsync(filePath),
            _ => throw new Exception("Unsupported resume format.")
        };
    }

    private Task<string> ExtractPdfTextAsync(string filePath)
    {
        var text = new StringBuilder();

        using (var document = PdfDocument.Open(filePath))
        {
            foreach (var page in document.GetPages())
            {
                text.AppendLine(
                    ContentOrderTextExtractor.GetText(page));

                text.AppendLine();
            }
        }

        return Task.FromResult(text.ToString());
    }

    private Task<string> ExtractDocxTextAsync(string filePath)
    {
        using var document = WordprocessingDocument.Open(filePath, false);

        var body = document.MainDocumentPart?.Document.Body;

        return Task.FromResult(body?.InnerText ?? string.Empty);
    }
}