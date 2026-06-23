using System;
using System.Threading.Tasks;
using MalakaBookFestManagement_GUI.Services;

namespace MalakaBookFestManagement_GUI.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _fullName = string.Empty;
        private bool _isRegisterMode;
        private string _errorMessage = string.Empty;
        private bool _isLoading;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public bool IsRegisterMode
        {
            get => _isRegisterMode;
            set
            {
                if (SetProperty(ref _isRegisterMode, value))
                {
                    ErrorMessage = string.Empty;
                    Password = string.Empty;
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public RelayCommand SubmitCommand { get; }
        public RelayCommand ToggleModeCommand { get; }

        public event Action? OnLoginSuccess;

        public LoginViewModel()
        {
            SubmitCommand = new RelayCommand(async () => await ExecuteSubmitAsync(), () => !IsLoading);
            ToggleModeCommand = new RelayCommand(() => IsRegisterMode = !IsRegisterMode);
        }

        private async Task ExecuteSubmitAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email and Password are required.";
                return;
            }

            if (IsRegisterMode && string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Full Name is required for registration.";
                return;
            }

            ErrorMessage = string.Empty;
            IsLoading = true;

            try
            {
                if (IsRegisterMode)
                {
                    var response = await ApiService.Instance.RegisterAsync(Email, Password, FullName);
                    if (response.Success)
                    {
                        OnLoginSuccess?.Invoke();
                    }
                    else
                    {
                        ErrorMessage = response.Message ?? "Registration failed.";
                    }
                }
                else
                {
                    var response = await ApiService.Instance.LoginAsync(Email, Password);
                    if (response.Success)
                    {
                        OnLoginSuccess?.Invoke();
                    }
                    else
                    {
                        ErrorMessage = response.Message ?? "Invalid email or password.";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
