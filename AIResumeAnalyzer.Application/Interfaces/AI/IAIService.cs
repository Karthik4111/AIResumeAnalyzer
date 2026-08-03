using AIResumeAnalyzer.Application.DTOs.AI;
using AIResumeAnalyzer.Application.DTOs.Interview;
using global::AIResumeAnalyzer.Application.DTOs.Interview;

namespace AIResumeAnalyzer.Application.Interfaces.AI;

public interface IAIService
{
    Task<AIAnalysisResponse> AnalyzeResumeAsync(AIAnalysisRequest request);

    Task<InterviewQuestionResponse> GenerateInterviewQuestionsAsync(InterviewQuestionRequest request);

    Task<InterviewQuestionResponse> GetInterviewQuestionsAsync(Guid resumeId);
}