using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalakaBookFestManagementSystem_GUI.Models;
using MalakaBookFestManagementSystem_GUI.Services;

namespace MalakaBookFestManagementSystem_GUI.ViewModels
{
    public partial class TalkshowViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private ObservableCollection<TalkshowDto> _talkshows = new();

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isSidePanelOpen;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private bool _isDetailMode;

        [ObservableProperty]
        private bool _isNotDetailMode = true;

        [ObservableProperty]
        private string _sidePanelTitle = "ADD TALKSHOW";

        [ObservableProperty]
        private TalkshowDto _currentTalkshow = new();

        public TalkshowViewModel()
        {
            _apiClient = new ApiClient();
            LoadTalkshowsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadTalkshowsAsync()
        {
            IsLoading = true;
            Talkshows.Clear();
            
            var data = await _apiClient.GetTalkshowsAsync();
            foreach (var item in data)
            {
                Talkshows.Add(item);
            }
            
            IsLoading = false;
        }

        [RelayCommand]
        private void PrepareAdd()
        {
            CurrentTalkshow = new TalkshowDto();
            IsEditMode = false;
            IsDetailMode = false;
            IsNotDetailMode = true;
            SidePanelTitle = "ADD TALKSHOW";
            IsSidePanelOpen = true;
        }

        [RelayCommand]
        private void PrepareEdit(TalkshowDto talkshow)
        {
            if (talkshow == null) return;
            CurrentTalkshow = new TalkshowDto
            {
                TalkshowId = talkshow.TalkshowId,
                Title = talkshow.Title,
                SpeakerName = talkshow.SpeakerName,
                StartTime = talkshow.StartTime,
                EndTime = talkshow.EndTime,
                Venue = talkshow.Venue,
                MaxCapacity = talkshow.MaxCapacity,
                RegisteredCount = talkshow.RegisteredCount
            };
            IsEditMode = true;
            IsDetailMode = false;
            IsNotDetailMode = true;
            SidePanelTitle = "EDIT TALKSHOW";
            IsSidePanelOpen = true;
        }

        [RelayCommand]
        private void ShowDetails(TalkshowDto talkshow)
        {
            if (talkshow == null) return;
            CurrentTalkshow = talkshow;
            IsEditMode = false;
            IsDetailMode = true;
            IsNotDetailMode = false;
            SidePanelTitle = "TALKSHOW DETAILS";
            IsSidePanelOpen = true;
        }

        [RelayCommand]
        private void CloseSidePanel()
        {
            IsSidePanelOpen = false;
        }

        [RelayCommand]
        private async Task SaveTalkshowAsync()
        {
            IsLoading = true;
            bool success = false;

            if (IsEditMode)
            {
                success = await _apiClient.UpdateTalkshowAsync(CurrentTalkshow.TalkshowId, CurrentTalkshow);
            }
            else
            {
                success = await _apiClient.CreateTalkshowAsync(CurrentTalkshow);
            }
            IsLoading = false;

            if (success)
            {
                IsSidePanelOpen = false;
                await LoadTalkshowsAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteTalkshowAsync(TalkshowDto talkshow)
        {
            if (talkshow == null) return;
            IsLoading = true;
            var success = await _apiClient.DeleteTalkshowAsync(talkshow.TalkshowId);
            IsLoading = false;

            if (success)
            {
                await LoadTalkshowsAsync();
            }
        }
    }
}
