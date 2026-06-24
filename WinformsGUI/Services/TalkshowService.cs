using WinformsGUI.Models;
using WinformsGUI.Utils;

namespace WinformsGUI.Services
{
    public class TalkshowService
    {
        public async Task<List<TalkshowResponse>?> GetAllTalkshowsAsync()
        {
            return await ApiClient.GetAsync<List<TalkshowResponse>>("Talkshow");
        }

        public async Task<TalkshowResponse?> GetTalkshowByIdAsync(Guid id)
        {
            return await ApiClient.GetAsync<TalkshowResponse>($"Talkshow/{id}");
        }

        public async Task<TalkshowResponse?> CreateTalkshowAsync(CreateTalkshowRequest request)
        {
            return await ApiClient.PostAsync<CreateTalkshowRequest, TalkshowResponse>("Talkshow", request);
        }

        public async Task<TalkshowResponse?> UpdateTalkshowAsync(Guid id, UpdateTalkshowRequest request)
        {
            return await ApiClient.PutAsync<UpdateTalkshowRequest, TalkshowResponse>($"Talkshow/{id}", request);
        }
    }
}
