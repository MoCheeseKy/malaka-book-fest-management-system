using System;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Views
{
    public partial class LoginView : UserControl
    {
        // Events for Main Window navigation routing
        public event Action OnLoginSuccess;
        public event Action OnNavigateRegister;

        public LoginView()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Mocking a successful login
            OnLoginSuccess?.Invoke();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Register view
            OnNavigateRegister?.Invoke();
        }
    }
}
