using AIResumeAnalyzer.Application.DTOs.Auth;
using AIResumeAnalyzer.Application.Interfaces.Auth;
using AIResumeAnalyzer.Application.Interfaces.Persistence;
using AIResumeAnalyzer.Application.Interfaces.Repositories;
using AIResumeAnalyzer.Domain.Entities;

namespace AIResumeAnalyzer.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check whether the email already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser != null)
            throw new Exception("User already exists.");

        // Get default role
        var allRoles = await _roleRepository.GetAllAsync();

        var role = allRoles.FirstOrDefault(r => r.Name == "Candidate");

        if (role == null)
        {
            throw new Exception($"Roles Count = {allRoles.Count()}");
        }

        if (role == null)
            throw new Exception("Candidate role not found.");

        // Create User
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),

            RoleId = role.Id,
            Role = role,

            CreatedOnUtc = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        await _unitOfWork.SaveChangesAsync();

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),

            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = role.Name
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
            throw new Exception("Invalid email or password.");

        // Verify password
        bool isValid = _passwordHasher.VerifyPassword(
            request.Password,
            user.PasswordHash);

        if (!isValid)
            throw new Exception("Invalid email or password.");

        // Generate JWT
        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),

            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role.Name
        };
    }
}