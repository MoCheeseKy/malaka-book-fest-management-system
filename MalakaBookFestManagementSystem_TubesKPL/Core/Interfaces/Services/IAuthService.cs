using MalakaBookFest.Core.Entities;

namespace MalakaBookFest.Core.Interfaces.Services;

public interface IAuthService
{
    Task<(string Token, User User)> LoginAsync(string email, string password);
    Task<User> RegisterAsync(string email, string password, string fullName);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
