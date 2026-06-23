using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MalakaBookFestManagementSystem_GUI.Models;

namespace MalakaBookFestManagementSystem_GUI.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        // Hardcode a dummy token or leave empty if user configures it later.
        public static string DummyToken { get; set; } = string.Empty;

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5278/");
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task EnsureAuthenticatedAsync()
        {
            if (!string.IsNullOrEmpty(DummyToken)) return;

            try
            {
                var loginData = new { Email = "admin2@malaka.com", Password = "" };
                var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
                
                // Try login
                var response = await _httpClient.PostAsync("api/Auth/login", content);
                if (!response.IsSuccessStatusCode)
                {
                    // If login fails, try register
                    var regData = new { Email = "admin2@malaka.com", Password = "Password123!", FullName = "Admin UI" };
                    var regContent = new StringContent(JsonSerializer.Serialize(regData), Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync("api/Auth/register", regContent);
                }

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    // Basic extraction without strict DTO matching
                    using var doc = System.Text.Json.JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("data", out var data) && data.TryGetProperty("token", out var token))
                    {
                        DummyToken = token.GetString() ?? "";
                    }
                }
            }
            catch { }
        }

        private async Task AttachTokenAsync()
        {
            await EnsureAuthenticatedAsync();
            if (!string.IsNullOrEmpty(DummyToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", DummyToken);
            }
        }

        public async Task<List<BoothDto>> GetBoothsAsync()
        {
            try
            {
                await AttachTokenAsync();
                var response = await _httpClient.GetAsync("api/Booth");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<BoothDto>>>(json, _jsonOptions);
                return apiResponse?.Data ?? new List<BoothDto>();
            }
            catch (Exception ex)
            {
                // In a real app, log the exception
                return new List<BoothDto>();
            }
        }

        public async Task<List<BookDto>> GetBooksByBoothAsync(Guid boothId)
        {
            try
            {
                await AttachTokenAsync();
                var response = await _httpClient.GetAsync($"api/booths/{boothId}/books");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<BookDto>>>(json, _jsonOptions);
                return apiResponse?.Data ?? new List<BookDto>();
            }
            catch (Exception ex)
            {
                return new List<BookDto>();
            }
        }

        public async Task<List<TalkshowDto>> GetTalkshowsAsync()
        {
            try
            {
                await AttachTokenAsync();
                var response = await _httpClient.GetAsync("api/Talkshow");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<TalkshowDto>>>(json, _jsonOptions);
                return apiResponse?.Data ?? new List<TalkshowDto>();
            }
            catch (Exception ex)
            {
                return new List<TalkshowDto>();
            }
        }

        public async Task<bool> CreateBoothAsync(BoothDto booth)
        {
            await AttachTokenAsync();
            var content = new StringContent(JsonSerializer.Serialize(booth), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Booth", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBoothAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _httpClient.DeleteAsync($"api/Booth/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateBoothAsync(Guid id, BoothDto booth)
        {
            await AttachTokenAsync();
            var content = new StringContent(JsonSerializer.Serialize(booth), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/Booth/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateBookAsync(Guid boothId, BookDto book)
        {
            await AttachTokenAsync();
            var content = new StringContent(JsonSerializer.Serialize(book), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/booths/{boothId}/books", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateBookAsync(Guid boothId, Guid bookId, BookDto book)
        {
            await AttachTokenAsync();
            var content = new StringContent(JsonSerializer.Serialize(book), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/booths/{boothId}/books/{bookId}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBookAsync(Guid boothId, Guid bookId)
        {
            await AttachTokenAsync();
            var response = await _httpClient.DeleteAsync($"api/booths/{boothId}/books/{bookId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateTalkshowAsync(TalkshowDto talkshow)
        {
            await AttachTokenAsync();
            var content = new StringContent(JsonSerializer.Serialize(talkshow), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Talkshow", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTalkshowAsync(Guid id, TalkshowDto talkshow)
        {
            await AttachTokenAsync();
            var content = new StringContent(JsonSerializer.Serialize(talkshow), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/Talkshow/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTalkshowAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _httpClient.DeleteAsync($"api/Talkshow/{id}");
            return response.IsSuccessStatusCode;
        }

        // Just fetching ticket prices as a public endpoint example
        public async Task<Dictionary<string, decimal>> GetTicketPricesAsync()
        {
            try
            {
                await AttachTokenAsync();
                var response = await _httpClient.GetAsync("api/Ticket/prices");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<Dictionary<string, decimal>>>(json, _jsonOptions);
                return apiResponse?.Data ?? new Dictionary<string, decimal>(); 
            }
            catch (Exception)
            {
                return new Dictionary<string, decimal>();
            }
        }
    }
}
