using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace WinformsGUI.Utils
{
    public static class ApiClient
    {
        private static readonly HttpClient _client = new HttpClient();

        public static HttpClient Client
        {
            get
            {
                if (!string.IsNullOrEmpty(Config.JwtToken))
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Config.JwtToken);
                }
                return _client;
            }
        }

        private static async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    try
                    {
                        var apiError = JsonSerializer.Deserialize<Models.ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (apiError != null && !string.IsNullOrEmpty(apiError.Message))
                        {
                            throw new Exception(apiError.Message);
                        }
                    }
                    catch { /* ignore json parse errors for fallback */ }
                }
                response.EnsureSuccessStatusCode(); // Fallback to default exception
            }
        }

        public static async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await Client.GetAsync(Config.ApiBaseUrl + endpoint);
            await EnsureSuccessOrThrowAsync(response);
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<Models.ApiResponse<T>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return apiResponse != null ? apiResponse.Data : default;
        }

        public static async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(Config.ApiBaseUrl + endpoint, content);
            await EnsureSuccessOrThrowAsync(response);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(responseContent)) return default;
            
            var apiResponse = JsonSerializer.Deserialize<Models.ApiResponse<TResponse>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return apiResponse != null ? apiResponse.Data : default;
        }

        public static async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await Client.PutAsync(Config.ApiBaseUrl + endpoint, content);
            await EnsureSuccessOrThrowAsync(response);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(responseContent)) return default;

            var apiResponse = JsonSerializer.Deserialize<Models.ApiResponse<TResponse>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return apiResponse != null ? apiResponse.Data : default;
        }

        public static async Task DeleteAsync(string endpoint)
        {
            var response = await Client.DeleteAsync(Config.ApiBaseUrl + endpoint);
            await EnsureSuccessOrThrowAsync(response);
        }
    }
}
