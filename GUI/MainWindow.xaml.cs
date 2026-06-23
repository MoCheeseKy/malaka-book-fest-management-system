using System.Windows;
using System.Windows.Controls;
using GUI.Views;

namespace GUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Start with Auth Flow
            ShowLoginView();
        }

        private void ShowLoginView()
        {
            SidebarBorder.Visibility = Visibility.Collapsed;
            var loginView = new LoginView();
            loginView.OnLoginSuccess += () => 
            {
                SidebarBorder.Visibility = Visibility.Visible;
                NavigateTo("Booths");
            };
            loginView.OnNavigateRegister += ShowRegisterView;
            MainContentControl.Content = loginView;
        }

        private void ShowRegisterView()
        {
            SidebarBorder.Visibility = Visibility.Collapsed;
            var registerView = new RegisterView();
            registerView.OnRegisterSuccess += ShowLoginView;
            registerView.OnNavigateLogin += ShowLoginView;
            MainContentControl.Content = registerView;
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                NavigateTo(tag);
            }
        }

        public void NavigateTo(string viewName, object? parameter = null)
        {
            UserControl? view = null;

            switch (viewName)
            {
                case "Booths":
                    var boothView = new BoothView();
                    boothView.BoothSelectedForBooks += (id) => NavigateTo("Books", id);
                    view = boothView;
                    break;
                case "Books":
                    // If parameter is a Booth ID
                    if (parameter is System.Guid boothId)
                    {
                        var bookView = new BookView();
                        bookView.LoadBooksForBooth(boothId);
                        view = bookView;
                    }
                    break;
                case "Talkshows":
                    view = new TalkshowView();
                    break;
                case "Tickets":
                    view = new TicketView();
                    break;
            }

            if (view != null)
            {
                MainContentControl.Content = view;
            }
        }
    }
}