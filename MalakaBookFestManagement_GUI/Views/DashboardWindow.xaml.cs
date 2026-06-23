using System;
using System.Windows;
using System.Windows.Input;
using MalakaBookFestManagement_GUI.ViewModels;

namespace MalakaBookFestManagement_GUI.Views
{
    public partial class DashboardWindow : Window
    {
        private readonly DashboardViewModel _viewModel;

        public DashboardWindow()
        {
            InitializeComponent();
            _viewModel = new DashboardViewModel();
            DataContext = _viewModel;

            _viewModel.OnLogout += ViewModel_OnLogout;
        }

        private void ViewModel_OnLogout()
        {
            Dispatcher.Invoke(() =>
            {
                var loginWindow = new MainWindow();
                loginWindow.Show();
                Application.Current.MainWindow = loginWindow;
                this.Close();
            });
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Only drag window if clicking on the background, not buttons or grids
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
