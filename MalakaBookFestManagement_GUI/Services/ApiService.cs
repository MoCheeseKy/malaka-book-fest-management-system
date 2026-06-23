using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MalakaBookFestManagement_GUI.Services
{
    public class ApiService
    {
        private static ApiService? _instance;
        public static ApiService Instance => _instance ??= new ApiService();

        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public string? Token { get; set; }
        public LoginResponseDto? CurrentUser { get; set; }

        private ApiService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5278/") // Default localhost backend port
            };
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task<ApiResponse<T>> SendAsync<T>(HttpMethod method, string path, object? body = null)
        {
            try
            {
                var request = new HttpRequestMessage(method, path);

                // Attach Bearer Token if available
                if (!string.IsNullOrEmpty(Token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                }

                if (body != null)
                {
                    request.Content = JsonContent.Create(body);
                }

                var response = await _client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
                    return result ?? ApiResponse<T>.Fail("Empty response from server.");
                }
                else
                {
                    try
                    {
                        var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
                        if (errorResult != null && !errorResult.Success)
                        {
                            return ApiResponse<T>.Fail(errorResult.Message ?? $"API error ({response.StatusCode})", errorResult.Errors);
                        }
                    }
                    catch
                    {
                        // Fallback if parsing fails
                    }
                    return ApiResponse<T>.Fail($"Server error: {response.StatusCode} ({response.ReasonPhrase})");
                }
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<T>.Fail($"Network error: Cannot connect to backend server. ({ex.Message})");
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.Fail($"An unexpected error occurred: {ex.Message}");
            }
        }

        // --- AUTHENTICATION ---
        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(string email, string password)
        {
            var dto = new LoginRequestDto { Email = email, Password = password };
            var response = await SendAsync<LoginResponseDto>(HttpMethod.Post, "api/auth/login", dto);
            if (response.Success && response.Data != null)
            {
                Token = response.Data.Token;
                CurrentUser = response.Data;
            }
            return response;
        }

        public async Task<ApiResponse<LoginResponseDto>> RegisterAsync(string email, string password, string fullName)
        {
            var dto = new RegisterRequestDto { Email = email, Password = password, FullName = fullName };
            var response = await SendAsync<LoginResponseDto>(HttpMethod.Post, "api/auth/register", dto);
            if (response.Success && response.Data != null)
            {
                Token = response.Data.Token;
                CurrentUser = response.Data;
            }
            return response;
        }

        public void Logout()
        {
            Token = null;
            CurrentUser = null;
        }

        // --- BOOTHS ---
        public async Task<ApiResponse<IEnumerable<BoothDto>>> GetBoothsAsync()
        {
            return await SendAsync<IEnumerable<BoothDto>>(HttpMethod.Get, "api/booth");
        }

        public async Task<ApiResponse<BoothDto>> CreateBoothAsync(CreateBoothDto dto)
        {
            return await SendAsync<BoothDto>(HttpMethod.Post, "api/booth", dto);
        }

        public async Task<ApiResponse<BoothDto>> UpdateBoothAsync(Guid id, UpdateBoothDto dto)
        {
            return await SendAsync<BoothDto>(HttpMethod.Put, $"api/booth/{id}", dto);
        }

        public async Task<ApiResponse<object>> DeleteBoothAsync(Guid id)
        {
            return await SendAsync<object>(HttpMethod.Delete, $"api/booth/{id}");
        }

        // --- BOOKS ---
        public async Task<ApiResponse<IEnumerable<BookDto>>> GetBooksByBoothAsync(Guid boothId)
        {
            return await SendAsync<IEnumerable<BookDto>>(HttpMethod.Get, $"api/booths/{boothId}/books");
        }

        public async Task<ApiResponse<BookDto>> AddBookAsync(Guid boothId, CreateBookDto dto)
        {
            return await SendAsync<BookDto>(HttpMethod.Post, $"api/booths/{boothId}/books", dto);
        }

        public async Task<ApiResponse<BookDto>> UpdateBookAsync(Guid boothId, Guid bookId, UpdateBookDto dto)
        {
            return await SendAsync<BookDto>(HttpMethod.Put, $"api/booths/{boothId}/books/{bookId}", dto);
        }

        public async Task<ApiResponse<object>> DeleteBookAsync(Guid boothId, Guid bookId)
        {
            return await SendAsync<object>(HttpMethod.Delete, $"api/booths/{boothId}/books/{bookId}");
        }

        // --- TICKETS ---
        public async Task<ApiResponse<IEnumerable<TicketDto>>> GetMyTicketsAsync()
        {
            return await SendAsync<IEnumerable<TicketDto>>(HttpMethod.Get, "api/ticket/my");
        }

        public async Task<ApiResponse<TicketDto>> PurchaseTicketAsync(PurchaseTicketDto dto)
        {
            return await SendAsync<TicketDto>(HttpMethod.Post, "api/ticket/purchase", dto);
        }

        public async Task<ApiResponse<TicketDto>> CancelTicketAsync(Guid id)
        {
            return await SendAsync<TicketDto>(HttpMethod.Post, $"api/ticket/{id}/cancel");
        }

        public async Task<ApiResponse<TicketDto>> ScanTicketAsync(string qrCode)
        {
            // Note: query parameters in scan route
            return await SendAsync<TicketDto>(HttpMethod.Post, $"api/ticket/scan?qrCode={Uri.EscapeDataString(qrCode)}");
        }

        // --- TALKSHOWS ---
        public async Task<ApiResponse<IEnumerable<TalkshowDto>>> GetTalkshowsAsync()
        {
            return await SendAsync<IEnumerable<TalkshowDto>>(HttpMethod.Get, "api/talkshow");
        }

        public async Task<ApiResponse<TalkshowDto>> CreateTalkshowAsync(CreateTalkshowDto dto)
        {
            return await SendAsync<TalkshowDto>(HttpMethod.Post, "api/talkshow", dto);
        }

        public async Task<ApiResponse<TalkshowDto>> UpdateTalkshowAsync(Guid id, UpdateTalkshowDto dto)
        {
            return await SendAsync<TalkshowDto>(HttpMethod.Put, $"api/talkshow/{id}", dto);
        }

        public async Task<ApiResponse<object>> RegisterTalkshowAsync(Guid id)
        {
            return await SendAsync<object>(HttpMethod.Post, $"api/talkshow/{id}/register");
        }

        public async Task<ApiResponse<object>> AdvanceTalkshowStatusAsync(Guid id)
        {
            return await SendAsync<object>(HttpMethod.Post, $"api/talkshow/{id}/advance-status");
        }

        // --- USERS & ROLES ---
        public async Task<ApiResponse<IEnumerable<UserDto>>> GetUsersAsync()
        {
            return await SendAsync<IEnumerable<UserDto>>(HttpMethod.Get, "api/user");
        }

        public async Task<ApiResponse<UserDto>> UpdateUserRoleAsync(Guid id, string role)
        {
            var dto = new UpdateUserRoleDto { Role = role };
            return await SendAsync<UserDto>(HttpMethod.Put, $"api/user/{id}/role", dto);
        }

        public async Task<ApiResponse<UserDto>> UpdateUserStatusAsync(Guid id, bool isActive)
        {
            var dto = new UpdateUserStatusDto { IsActive = isActive };
            return await SendAsync<UserDto>(HttpMethod.Put, $"api/user/{id}/status", dto);
        }
    }

    // Helper class for local static API response instantiation
    public static class ApiResponse
    {
        public static ApiResponse<T> Fail<T>(string message, List<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };
    }
}
