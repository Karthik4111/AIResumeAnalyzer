using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIResumeAnalyzer.Application.DTOs.Pdf;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Application.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AIResumeAnalyzer.Infrastructure.Services.Pdf;

public class PdfService : IPdfService
{
    private readonly IATSReportRepository _atsReportRepository;
    private readonly ICoverLetterRepository _coverLetterRepository;
    private readonly IInterviewQuestionRepository _interviewQuestionRepository;

    public PdfService(
        IATSReportRepository atsReportRepository,
        ICoverLetterRepository coverLetterRepository,
        IInterviewQuestionRepository interviewQuestionRepository)
    {
        _atsReportRepository = atsReportRepository;
        _coverLetterRepository = coverLetterRepository;
        _interviewQuestionRepository = interviewQuestionRepository;

        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<PdfResponse> ExportATSReportAsync(Guid atsReportId)
    {
        var report = await _atsReportRepository.GetByIdAsync(atsReportId);

        if (report == null)
            throw new Exception("ATS Report not found.");

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header()
                    .Text("AI Resume Analyzer")
                    .FontSize(24)
                    .Bold();

                page.Content()
                    .Column(column =>
                    {
                        column.Item().Text("ATS REPORT")
                            .FontSize(20)
                            .Bold();

                        column.Item().PaddingTop(10);

                        column.Item().Text($"ATS Score: {report.AtsScore}");

                        column.Item().PaddingTop(10);

                        column.Item().Text(report.Summary);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated on ");
                        x.Span(DateTime.UtcNow.ToString("dd MMM yyyy HH:mm"));
                    });
            });
        }).GeneratePdf();

        return new PdfResponse
        {
            FileBytes = pdf,
            FileName = "ATS_Report.pdf"
        };
    }

    public async Task<PdfResponse> ExportCoverLetterAsync(Guid coverLetterId)
    {
        var coverLetter =
            await _coverLetterRepository.GetByIdAsync(coverLetterId);

        if (coverLetter == null)
            throw new Exception("Cover Letter not found.");

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header()
                    .Text("AI Resume Analyzer")
                    .FontSize(24)
                    .Bold();

                page.Content()
                    .Column(column =>
                    {
                        column.Item()
                            .Text("Cover Letter")
                            .FontSize(20)
                            .Bold();

                        column.Item().PaddingTop(15);

                        column.Item().Text(coverLetter.Content);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text($"Generated on {DateTime.UtcNow:dd MMM yyyy HH:mm}");
            });
        }).GeneratePdf();

        return new PdfResponse
        {
            FileBytes = pdf,
            FileName = "CoverLetter.pdf"
        };
    }

    public async Task<PdfResponse> ExportInterviewQuestionsAsync(Guid atsReportId)
    {
        var questions =
            await _interviewQuestionRepository
                .GetByATSReportIdAsync(atsReportId);

        if (!questions.Any())
            throw new Exception("Interview Questions not found.");

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header()
                    .Text("AI Resume Analyzer")
                    .FontSize(24)
                    .Bold();

                page.Content()
                    .Column(column =>
                    {
                        column.Item()
                            .Text("Interview Questions")
                            .FontSize(20)
                            .Bold();

                        column.Item().PaddingTop(15);

                        foreach (var question in questions)
                        {
                            column.Item()
                                .PaddingBottom(8)
                                .Text("• " + question.Question);
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text($"Generated on {DateTime.UtcNow:dd MMM yyyy HH:mm}");
            });
        }).GeneratePdf();

        return new PdfResponse
        {
            FileBytes = pdf,
            FileName = "InterviewQuestions.pdf"
        };
    }
}