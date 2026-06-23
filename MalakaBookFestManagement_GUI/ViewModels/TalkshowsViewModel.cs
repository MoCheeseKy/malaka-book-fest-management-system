using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MalakaBookFestManagement_GUI.Services;

namespace MalakaBookFestManagement_GUI.ViewModels
{
    public class TalkshowsViewModel : ViewModelBase
    {
        private ObservableCollection<TalkshowDto> _talkshows = new();
        private TalkshowDto? _selectedTalkshow;

        // Form Fields
        private string _title = string.Empty;
        private string _speakerName = string.Empty;
        private string _speakerBio = string.Empty;
        private string _venue = string.Empty;
        private DateTime _startTime = DateTime.Today.AddHours(9);
        private DateTime _endTime = DateTime.Today.AddHours(11);
        private int _maxCapacity = 100;

        private bool _isLoading;
        private string _statusMessage = string.Empty;
        private bool _isSuccessMessage;
        private bool _isEditMode;

        public ObservableCollection<TalkshowDto> Talkshows
        {
            get => _talkshows;
            set => SetProperty(ref _talkshows, value);
        }

        public TalkshowDto? SelectedTalkshow
        {
            get => _selectedTalkshow;
            set
            {
                if (SetProperty(ref _selectedTalkshow, value))
                {
                    if (value != null)
                    {
                        Title = value.Title;
                        SpeakerName = value.SpeakerName;
                        SpeakerBio = value.SpeakerBio;
                        Venue = value.Venue;
                        StartTime = value.StartTime;
                        EndTime = value.EndTime;
                        MaxCapacity = value.MaxCapacity;
                        IsEditMode = true;
                    }
                    else
                    {
                        ResetForm();
                    }
                    OnPropertyChanged(nameof(CanRegister));
                }
            }
        }

        // Form Bindings
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string SpeakerName
        {
            get => _speakerName;
            set => SetProperty(ref _speakerName, value);
        }

        public string SpeakerBio
        {
            get => _speakerBio;
            set => SetProperty(ref _speakerBio, value);
        }

        public string Venue
        {
            get => _venue;
            set => SetProperty(ref _venue, value);
        }

        public DateTime StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        public DateTime EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }

        public int MaxCapacity
        {
            get => _maxCapacity;
            set => SetProperty(ref _maxCapacity, value);
        }

        // UI States
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsSuccessMessage
        {
            get => _isSuccessMessage;
            set => SetProperty(ref _isSuccessMessage, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        // Authorizations
        public bool IsAdmin => ApiService.Instance.CurrentUser?.Role == "Admin";
        public bool CanEdit => IsAdmin;
        public bool CanRegister => SelectedTalkshow != null && SelectedTalkshow.Status.Equals("Scheduled", StringComparison.OrdinalIgnoreCase);

        public RelayCommand SaveCommand { get; }
        public RelayCommand RegisterCommand { get; }
        public RelayCommand AdvanceStatusCommand { get; }
        public RelayCommand ClearFormCommand { get; }
        public RelayCommand LoadTalkshowsCommand { get; }

        public TalkshowsViewModel()
        {
            SaveCommand = new RelayCommand(async () => await SaveTalkshowAsync(), () => CanEdit && !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(SpeakerName));
            RegisterCommand = new RelayCommand(async () => await RegisterAttendeeAsync(), () => CanRegister);
            AdvanceStatusCommand = new RelayCommand(async () => await AdvanceStatusAsync(), () => CanEdit && SelectedTalkshow != null);
            ClearFormCommand = new RelayCommand(ResetForm);
            LoadTalkshowsCommand = new RelayCommand(async () => await LoadTalkshowsAsync());

            _ = LoadTalkshowsAsync();
        }

        private async Task LoadTalkshowsAsync()
        {
            IsLoading = true;
            try
            {
                var response = await ApiService.Instance.GetTalkshowsAsync();
                if (response.Success && response.Data != null)
                {
                    Talkshows = new ObservableCollection<TalkshowDto>(response.Data);
                }
                else
                {
                    ShowError(response.Message ?? "Failed to load talkshows.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading talkshows: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SaveTalkshowAsync()
        {
            IsLoading = true;
            try
            {
                if (IsEditMode && SelectedTalkshow != null)
                {
                    var dto = new UpdateTalkshowDto
                    {
                        Title = Title,
                        SpeakerName = SpeakerName,
                        SpeakerBio = SpeakerBio,
                        Venue = Venue,
                        StartTime = StartTime,
                        EndTime = EndTime,
                        MaxCapacity = MaxCapacity
                    };

                    var response = await ApiService.Instance.UpdateTalkshowAsync(SelectedTalkshow.TalkshowId, dto);
                    if (response.Success)
                    {
                        ShowSuccess("Talkshow updated successfully.");
                        ResetForm();
                        await LoadTalkshowsAsync();
                    }
                    else
                    {
                        ShowError(response.Message ?? "Failed to update talkshow.");
                    }
                }
                else
                {
                    var dto = new CreateTalkshowDto
                    {
                        Title = Title,
                        SpeakerName = SpeakerName,
                        SpeakerBio = SpeakerBio,
                        Venue = Venue,
                        StartTime = StartTime,
                        EndTime = EndTime,
                        MaxCapacity = MaxCapacity
                    };

                    var response = await ApiService.Instance.CreateTalkshowAsync(dto);
                    if (response.Success)
                    {
                        ShowSuccess("Talkshow created successfully.");
                        ResetForm();
                        await LoadTalkshowsAsync();
                    }
                    else
                    {
                        ShowError(response.Message ?? "Failed to create talkshow.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error saving talkshow: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RegisterAttendeeAsync()
        {
            if (SelectedTalkshow == null) return;

            IsLoading = true;
            try
            {
                var response = await ApiService.Instance.RegisterTalkshowAsync(SelectedTalkshow.TalkshowId);
                if (response.Success)
                {
                    ShowSuccess("Successfully registered for talkshow!");
                    await LoadTalkshowsAsync(); // Refresh count
                }
                else
                {
                    ShowError(response.Message ?? "Failed to register for talkshow.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error registering: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AdvanceStatusAsync()
        {
            if (SelectedTalkshow == null) return;

            IsLoading = true;
            try
            {
                var response = await ApiService.Instance.AdvanceTalkshowStatusAsync(SelectedTalkshow.TalkshowId);
                if (response.Success)
                {
                    ShowSuccess("Talkshow status advanced successfully.");
                    await LoadTalkshowsAsync();
                }
                else
                {
                    ShowError(response.Message ?? "Failed to advance talkshow status.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error advancing status: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ResetForm()
        {
            Title = string.Empty;
            SpeakerName = string.Empty;
            SpeakerBio = string.Empty;
            Venue = string.Empty;
            StartTime = DateTime.Today.AddHours(9);
            EndTime = DateTime.Today.AddHours(11);
            MaxCapacity = 100;
            IsEditMode = false;
            _selectedTalkshow = null;
            OnPropertyChanged(nameof(SelectedTalkshow));
        }

        private void ShowError(string message)
        {
            StatusMessage = message;
            IsSuccessMessage = false;
        }

        private void ShowSuccess(string message)
        {
            StatusMessage = message;
            IsSuccessMessage = true;
        }
    }
}
