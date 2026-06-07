using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Enums;
using MalakaBookFest.Core.Interfaces.Repositories;
using MalakaBookFest.Core.Interfaces.Services;
using MalakaBookFest.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace MalakaBookFest.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtConfig _jwtConfig;
    public AuthService(IUserRepository userRepository, IOptions<JwtConfig> jwtOptions)
    {
        _userRepository = userRepository;
        _jwtConfig = jwtOptions.Value;
    }
    public async Task<(string Token, User User)> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");
        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is deactivated.");
        if (!VerifyPassword(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");
        var token = GenerateJwtToken(user);
        return (token, user);
    }
    public async Task<User> RegisterAsync(string email, string password, string fullName)
    {
        if (await _userRepository.EmailExistsAsync(email))
            throw new InvalidOperationException("Email is already registered.");
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = email,
            PasswordHash = HashPassword(password),
            FullName = fullName,
            Role = UserRole.Attendee,
        };
        return await _userRepository.CreateAsync(user);
    }
    public string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    public bool VerifyPassword(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("fullName", user.FullName),
        };
        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiryMinutes),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}