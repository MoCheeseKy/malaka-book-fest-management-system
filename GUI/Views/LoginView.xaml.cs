using System;
using System.Windows;
using System.Windows.Controls;
using GUI.Services;

namespace GUI.Views
{
    public partial class LoginView : UserControl
    {
        // Events for Main Window navigation routing
        public event Action? OnLoginSuccess;
        public event Action? OnNavigateRegister;

        public LoginView()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var email = TxtEmail.Text.Trim();
            var password = TxtPassword.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both email and password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var apiService = new ApiService();
                var response = await apiService.LoginAsync(email, password);
                if (response != null)
                {
                    OnLoginSuccess?.Invoke();
                }
                else
                {
                    MessageBox.Show("Login failed. No response from server.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Register view
            OnNavigateRegister?.Invoke();
        }
    }
}
