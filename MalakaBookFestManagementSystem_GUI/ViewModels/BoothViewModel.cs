using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalakaBookFestManagementSystem_GUI.Models;
using MalakaBookFestManagementSystem_GUI.Services;

namespace MalakaBookFestManagementSystem_GUI.ViewModels
{
    public partial class BoothViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private ObservableCollection<BoothDto> _booths = new();

        [ObservableProperty]
        private BoothDto? _selectedBooth;

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
        private string _sidePanelTitle = "ADD BOOTH";

        [ObservableProperty]
        private BoothDto _currentBooth = new();

        public BoothViewModel()
        {
            _apiClient = new ApiClient();
            LoadBoothsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadBoothsAsync()
        {
            IsLoading = true;
            Booths.Clear();
            
            var data = await _apiClient.GetBoothsAsync();
            foreach (var item in data)
            {
                Booths.Add(item);
            }
            
            IsLoading = false;
        }

        [RelayCommand]
        private void PrepareAdd()
        {
            CurrentBooth = new BoothDto();
            IsEditMode = false;
            IsDetailMode = false;
            IsNotDetailMode = true;
            SidePanelTitle = "ADD BOOTH";
            IsSidePanelOpen = true;
        }

        [RelayCommand]
        private void PrepareEdit(BoothDto booth)
        {
            if (booth == null) return;
            // Clone the object so we don't modify the UI directly before saving
            CurrentBooth = new BoothDto
            {
                BoothId = booth.BoothId,
                BoothName = booth.BoothName,
                BoothNumber = booth.BoothNumber,
                Description = booth.Description,
                OrganizerId = booth.OrganizerId,
                Category = booth.Category,
                IsActive = booth.IsActive
            };
            IsEditMode = true;
            IsDetailMode = false;
            IsNotDetailMode = true;
            SidePanelTitle = "EDIT BOOTH";
            IsSidePanelOpen = true;
        }

        [RelayCommand]
        private void ShowDetails(BoothDto booth)
        {
            if (booth == null) return;
            CurrentBooth = booth;
            IsEditMode = false;
            IsDetailMode = true;
            IsNotDetailMode = false;
            SidePanelTitle = "BOOTH DETAILS";
            IsSidePanelOpen = true;
        }

        [RelayCommand]
        private void CloseSidePanel()
        {
            IsSidePanelOpen = false;
        }

        [RelayCommand]
        private async Task SaveBoothAsync()
        {
            IsLoading = true;
            bool success = false;

            if (IsEditMode)
            {
                success = await _apiClient.UpdateBoothAsync(CurrentBooth.BoothId, CurrentBooth);
            }
            else
            {
                success = await _apiClient.CreateBoothAsync(CurrentBooth);
            }

            IsLoading = false;

            if (success)
            {
                IsSidePanelOpen = false;
                await LoadBoothsAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteBoothAsync(BoothDto booth)
        {
            if (booth == null) return;

            IsLoading = true;
            var success = await _apiClient.DeleteBoothAsync(booth.BoothId);
            IsLoading = false;

            if (success)
            {
                await LoadBoothsAsync();
            }
        }
    }
}
