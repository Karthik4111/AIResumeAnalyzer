using AIResumeAnalyzer.Domain.Entities;

namespace AIResumeAnalyzer.Application.Interfaces.Auth;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}