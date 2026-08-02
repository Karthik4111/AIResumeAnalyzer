using AIResumeAnalyzer.Application.DTOs.Auth;

namespace AIResumeAnalyzer.Application.Interfaces.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse> LoginAsync(LoginRequest request);
}