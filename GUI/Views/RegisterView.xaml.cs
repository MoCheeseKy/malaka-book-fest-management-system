using System;
using System.Windows;
using System.Windows.Controls;
using GUI.Services;

namespace GUI.Views
{
    public partial class RegisterView : UserControl
    {
        // Events for Main Window navigation routing
        public event Action? OnRegisterSuccess;
        public event Action? OnNavigateLogin;

        public RegisterView()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var fullName = TxtFullName.Text.Trim();
            var email = TxtEmail.Text.Trim();
            var password = TxtPassword.Password;

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var apiService = new ApiService();
                var response = await apiService.RegisterAsync(email, password, fullName);
                if (response != null)
                {
                    MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    OnRegisterSuccess?.Invoke();
                }
                else
                {
                    MessageBox.Show("Registration failed. No response from server.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to Login view
            OnNavigateLogin?.Invoke();
        }
    }
}
