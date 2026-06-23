using System;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Views
{
    public partial class RegisterView : UserControl
    {
        // Events for Main Window navigation routing
        public event Action OnRegisterSuccess;
        public event Action OnNavigateLogin;

        public RegisterView()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Mocking a successful registration (could also just navigate back to login)
            OnRegisterSuccess?.Invoke();
        }

        private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to Login view
            OnNavigateLogin?.Invoke();
        }
    }
}
