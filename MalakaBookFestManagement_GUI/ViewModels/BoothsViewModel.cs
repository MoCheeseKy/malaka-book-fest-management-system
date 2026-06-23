using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MalakaBookFestManagement_GUI.Services;

namespace MalakaBookFestManagement_GUI.ViewModels
{
    public class BoothsViewModel : ViewModelBase
    {
        private ObservableCollection<BoothDto> _booths = new();
        private BoothDto? _selectedBooth;

        // Form Fields
        private string _boothName = string.Empty;
        private string _description = string.Empty;
        private string _boothNumber = string.Empty;
        private string _category = "Publisher";
        private bool _isActive = true;

        private List<string> _categories = new() { "Publisher", "IndieAuthor", "Merchandise", "FoodBeverage", "Other" };

        private bool _isLoading;
        private string _statusMessage = string.Empty;
        private bool _isSuccessMessage;
        private bool _isEditMode;

        public ObservableCollection<BoothDto> Booths
        {
            get => _booths;
            set => SetProperty(ref _booths, value);
        }

        public BoothDto? SelectedBooth
        {
            get => _selectedBooth;
            set
            {
                if (SetProperty(ref _selectedBooth, value))
                {
                    if (value != null)
                    {
                        BoothName = value.BoothName;
                        Description = value.Description;
                        BoothNumber = value.BoothNumber;
                        Category = value.Category;
                        IsActive = value.IsActive;
                        IsEditMode = true;
                    }
                    else
                    {
                        ResetForm();
                    }
                }
            }
        }

        // Form Bindings
        public string BoothName
        {
            get => _boothName;
            set => SetProperty(ref _boothName, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string BoothNumber
        {
            get => _boothNumber;
            set => SetProperty(ref _boothNumber, value);
        }

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public List<string> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
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

        // Role authorization check
        public bool CanEdit => ApiService.Instance.CurrentUser?.Role == "Admin" || ApiService.Instance.CurrentUser?.Role == "Organizer";

        public RelayCommand SaveCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ClearFormCommand { get; }

        public BoothsViewModel()
        {
            SaveCommand = new RelayCommand(async () => await SaveBoothAsync(), () => CanEdit && !string.IsNullOrWhiteSpace(BoothName) && !string.IsNullOrWhiteSpace(BoothNumber));
            DeleteCommand = new RelayCommand(async () => await DeleteBoothAsync(), () => CanEdit && SelectedBooth != null);
            ClearFormCommand = new RelayCommand(ResetForm);

            _ = LoadBoothsAsync();
        }

        private async Task LoadBoothsAsync()
        {
            IsLoading = true;
            try
            {
                var response = await ApiService.Instance.GetBoothsAsync();
                if (response.Success && response.Data != null)
                {
                    Booths = new ObservableCollection<BoothDto>(response.Data);
                }
                else
                {
                    ShowError(response.Message ?? "Failed to load booths.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading booths: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SaveBoothAsync()
        {
            IsLoading = true;
            try
            {
                if (IsEditMode && SelectedBooth != null)
                {
                    var dto = new UpdateBoothDto
                    {
                        BoothName = BoothName,
                        Description = Description,
                        Category = Category,
                        IsActive = IsActive
                    };

                    var response = await ApiService.Instance.UpdateBoothAsync(SelectedBooth.BoothId, dto);
                    if (response.Success)
                    {
                        ShowSuccess("Booth updated successfully.");
                        ResetForm();
                        await LoadBoothsAsync();
                    }
                    else
                    {
                        ShowError(response.Message ?? "Failed to update booth.");
                    }
                }
                else
                {
                    var dto = new CreateBoothDto
                    {
                        BoothName = BoothName,
                        Description = Description,
                        BoothNumber = BoothNumber,
                        Category = Category
                    };

                    var response = await ApiService.Instance.CreateBoothAsync(dto);
                    if (response.Success)
                    {
                        ShowSuccess("Booth created successfully.");
                        ResetForm();
                        await LoadBoothsAsync();
                    }
                    else
                    {
                        ShowError(response.Message ?? "Failed to create booth.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error saving booth: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteBoothAsync()
        {
            if (SelectedBooth == null) return;

            if (System.Windows.MessageBox.Show($"Are you sure you want to delete booth '{SelectedBooth.BoothName}'?", "Confirm Delete", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning) != System.Windows.MessageBoxResult.Yes)
            {
                return;
            }

            IsLoading = true;
            try
            {
                var response = await ApiService.Instance.DeleteBoothAsync(SelectedBooth.BoothId);
                if (response.Success)
                {
                    ShowSuccess("Booth deleted successfully.");
                    ResetForm();
                    await LoadBoothsAsync();
                }
                else
                {
                    ShowError(response.Message ?? "Failed to delete booth.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error deleting booth: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ResetForm()
        {
            BoothName = string.Empty;
            Description = string.Empty;
            BoothNumber = string.Empty;
            Category = "Publisher";
            IsActive = true;
            IsEditMode = false;
            _selectedBooth = null;
            OnPropertyChanged(nameof(SelectedBooth));
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
