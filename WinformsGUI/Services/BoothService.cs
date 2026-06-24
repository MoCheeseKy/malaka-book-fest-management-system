using WinformsGUI.Models;
using WinformsGUI.Utils;

namespace WinformsGUI.Services
{
    public class BoothService
    {
        public async Task<List<BoothResponse>?> GetAllBoothsAsync()
        {
            return await ApiClient.GetAsync<List<BoothResponse>>("Booth");
        }

        public async Task<BoothResponse?> GetBoothByIdAsync(Guid id)
        {
            return await ApiClient.GetAsync<BoothResponse>($"Booth/{id}");
        }

        public async Task<BoothResponse?> CreateBoothAsync(CreateBoothRequest request)
        {
            return await ApiClient.PostAsync<CreateBoothRequest, BoothResponse>("Booth", request);
        }

        public async Task<BoothResponse?> UpdateBoothAsync(Guid id, UpdateBoothRequest request)
        {
            return await ApiClient.PutAsync<UpdateBoothRequest, BoothResponse>($"Booth/{id}", request);
        }

        public async Task DeleteBoothAsync(Guid id)
        {
            await ApiClient.DeleteAsync($"Booth/{id}");
        }
    }
}
