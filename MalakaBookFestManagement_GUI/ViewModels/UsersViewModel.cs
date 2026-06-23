using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MalakaBookFestManagement_GUI.Services;

namespace MalakaBookFestManagement_GUI.ViewModels
{
    public class UsersViewModel : ViewModelBase
    {
        private ObservableCollection<UserDto> _users = new();
        private UserDto? _selectedUser;

        // Edit bindings
        private string _selectedRole = "Attendee";
        private bool _isActive = true;
        private List<string> _roles = new() { "Guest", "Attendee", "Organizer", "Admin" };

        private bool _isLoading;
        private string _statusMessage = string.Empty;
        private bool _isSuccessMessage;

        public ObservableCollection<UserDto> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public UserDto? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    if (value != null)
                    {
                        SelectedRole = value.Role;
                        IsActive = value.IsActive;
                    }
                }
            }
        }

        public string SelectedRole
        {
            get => _selectedRole;
            set => SetProperty(ref _selectedRole, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public List<string> Roles
        {
            get => _roles;
            set => SetProperty(ref _roles, value);
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

        public RelayCommand SaveChangesCommand { get; }
        public RelayCommand LoadUsersCommand { get; }

        public UsersViewModel()
        {
            SaveChangesCommand = new RelayCommand(async () => await SaveChangesAsync(), () => SelectedUser != null);
            LoadUsersCommand = new RelayCommand(async () => await LoadUsersAsync());

            _ = LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            IsLoading = true;
            try
            {
                var response = await ApiService.Instance.GetUsersAsync();
                if (response.Success && response.Data != null)
                {
                    // Exclude current user from list so they don't accidentally locked themselves out
                    var currentUserId = ApiService.Instance.CurrentUser?.UserId;
                    Users = new ObservableCollection<UserDto>(response.Data.Where(u => u.UserId != currentUserId));
                }
                else
                {
                    ShowError(response.Message ?? "Failed to load users.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading users: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SaveChangesAsync()
        {
            if (SelectedUser == null) return;

            IsLoading = true;
            try
            {
                bool roleSuccess = true;
                bool statusSuccess = true;
                string message = string.Empty;

                // Update Role if changed
                if (SelectedRole != SelectedUser.Role)
                {
                    var response = await ApiService.Instance.UpdateUserRoleAsync(SelectedUser.UserId, SelectedRole);
                    roleSuccess = response.Success;
                    if (!roleSuccess) message += response.Message + " ";
                }

                // Update Status if changed
                if (IsActive != SelectedUser.IsActive)
                {
                    var response = await ApiService.Instance.UpdateUserStatusAsync(SelectedUser.UserId, IsActive);
                    statusSuccess = response.Success;
                    if (!statusSuccess) message += response.Message + " ";
                }

                if (roleSuccess && statusSuccess)
                {
                    ShowSuccess("User updated successfully.");
                    SelectedUser = null;
                    await LoadUsersAsync();
                }
                else
                {
                    ShowError(string.IsNullOrWhiteSpace(message) ? "Failed to update user." : message.Trim());
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error saving changes: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
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
