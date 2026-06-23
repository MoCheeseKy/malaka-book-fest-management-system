using System;
using System.Windows;
using System.Windows.Input;
using MalakaBookFestManagement_GUI.ViewModels;
using MalakaBookFestManagement_GUI.Views;

namespace MalakaBookFestManagement_GUI
{
    public partial class MainWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;

            _viewModel.OnLoginSuccess += ViewModel_OnLoginSuccess;
        }

        private void ViewModel_OnLoginSuccess()
        {
            // Run on UI Thread
            Dispatcher.Invoke(() =>
            {
                var dashboard = new DashboardWindow();
                dashboard.Show();
                Application.Current.MainWindow = dashboard;
                this.Close();
            });
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Manually capture password from PasswordBox for security and simplicity
            _viewModel.Password = PasswordInput.Password;
            if (_viewModel.SubmitCommand.CanExecute(null))
            {
                _viewModel.SubmitCommand.Execute(null);
            }
        }
    }
}