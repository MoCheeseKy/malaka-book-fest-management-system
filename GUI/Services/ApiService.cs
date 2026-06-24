using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using GUI.Models;

namespace GUI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public static string? Token { get; set; }

        public ApiService()
        {
            // Base URL based on launchSettings.json of the API
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5278/api/") };
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            EnsureAuthHeader();
        }

        private void EnsureAuthHeader()
        {
            if (!string.IsNullOrEmpty(Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        // --- Booth ---
        public async Task<List<Booth>?> GetBoothsAsync()
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<List<Booth>>>("Booth", _jsonOptions);
            return res?.Data;
        }

        public async Task<Booth?> GetBoothAsync(Guid id)
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<Booth>>($"Booth/{id}", _jsonOptions);
            return res?.Data;
        }

        public async Task<Booth?> CreateBoothAsync(BoothCreateRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("Booth", request);
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadFromJsonAsync<ApiResponse<Booth>>(_jsonOptions);
            return res?.Data;
        }

        public async Task UpdateBoothAsync(Guid id, Booth booth)
        {
            var response = await _httpClient.PutAsJsonAsync($"Booth/{id}", booth);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteBoothAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"Booth/{id}");
            response.EnsureSuccessStatusCode();
        }

        // --- Book ---
        public async Task<List<Book>?> GetBooksByBoothAsync(Guid boothId)
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<List<Book>>>($"booths/{boothId}/books", _jsonOptions);
            return res?.Data;
        }

        public async Task<Book?> CreateBookAsync(Guid boothId, BookCreateRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"booths/{boothId}/books", request);
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadFromJsonAsync<ApiResponse<Book>>(_jsonOptions);
            return res?.Data;
        }

        public async Task UpdateBookAsync(Guid boothId, Guid bookId, Book book)
        {
            var response = await _httpClient.PutAsJsonAsync($"booths/{boothId}/books/{bookId}", book);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteBookAsync(Guid boothId, Guid bookId)
        {
            var response = await _httpClient.DeleteAsync($"booths/{boothId}/books/{bookId}");
            response.EnsureSuccessStatusCode();
        }

        // --- Talkshow ---
        public async Task<List<Talkshow>?> GetTalkshowsAsync()
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<List<Talkshow>>>("Talkshow", _jsonOptions);
            return res?.Data;
        }

        public async Task<Talkshow?> GetTalkshowAsync(Guid id)
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<Talkshow>>($"Talkshow/{id}", _jsonOptions);
            return res?.Data;
        }

        public async Task<Talkshow?> CreateTalkshowAsync(TalkshowCreateRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("Talkshow", request);
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadFromJsonAsync<ApiResponse<Talkshow>>(_jsonOptions);
            return res?.Data;
        }

        public async Task UpdateTalkshowAsync(Guid id, Talkshow talkshow)
        {
            var response = await _httpClient.PutAsJsonAsync($"Talkshow/{id}", talkshow);
            response.EnsureSuccessStatusCode();
        }

        public async Task RegisterTalkshowAsync(Guid id)
        {
            var response = await _httpClient.PostAsync($"Talkshow/{id}/register", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task AdvanceTalkshowStatusAsync(Guid id)
        {
            var response = await _httpClient.PostAsync($"Talkshow/{id}/advance-status", null);
            response.EnsureSuccessStatusCode();
        }

        // --- Ticket ---
        public async Task<Dictionary<string, decimal>?> GetTicketPricesAsync()
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<Dictionary<string, decimal>>>("Ticket/prices", _jsonOptions);
            return res?.Data;
        }

        public async Task<List<Ticket>?> GetMyTicketsAsync()
        {
            // This endpoint might need authentication
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<List<Ticket>>>("Ticket/my", _jsonOptions);
            return res?.Data;
        }

        public async Task<Ticket?> PurchaseTicketAsync(TicketType type)
        {
            var response = await _httpClient.PostAsJsonAsync("Ticket/purchase", new { Type = type });
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadFromJsonAsync<ApiResponse<Ticket>>(_jsonOptions);
            return res?.Data;
        }

        public async Task CancelTicketAsync(Guid id)
        {
            var response = await _httpClient.PostAsync($"Ticket/{id}/cancel", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Ticket?> ScanTicketAsync(string qrCode)
        {
            EnsureAuthHeader();
            var response = await _httpClient.PostAsync($"Ticket/scan?qrCode={Uri.EscapeDataString(qrCode)}", null);
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadFromJsonAsync<ApiResponse<Ticket>>(_jsonOptions);
            return res?.Data;
        }

        // --- Auth ---
        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("Auth/login", new { Email = email, Password = password });
            if (!response.IsSuccessStatusCode)
            {
                var errContent = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(_jsonOptions);
                throw new Exception(errContent?.Message ?? "Login failed.");
            }
            var res = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>(_jsonOptions);
            if (res?.Data != null)
            {
                Token = res.Data.Token;
                EnsureAuthHeader();
            }
            return res?.Data;
        }

        public async Task<LoginResponse?> RegisterAsync(string email, string password, string fullName)
        {
            var response = await _httpClient.PostAsJsonAsync("Auth/register", new { Email = email, Password = password, FullName = fullName });
            if (!response.IsSuccessStatusCode)
            {
                var errContent = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(_jsonOptions);
                throw new Exception(errContent?.Message ?? "Registration failed.");
            }
            var res = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>(_jsonOptions);
            if (res?.Data != null)
            {
                Token = res.Data.Token;
                EnsureAuthHeader();
            }
            return res?.Data;
        }
    }
}
