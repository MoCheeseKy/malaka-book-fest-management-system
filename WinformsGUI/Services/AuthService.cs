using WinformsGUI.Models;
using WinformsGUI.Utils;

namespace WinformsGUI.Services
{
    public class AuthService
    {
        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var response = await ApiClient.PostAsync<LoginRequest, AuthResponse>("Auth/login", request);
            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                Config.JwtToken = response.Token;
            }
            return response;
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            return await ApiClient.PostAsync<RegisterRequest, AuthResponse>("Auth/register", request);
        }
    }
}
