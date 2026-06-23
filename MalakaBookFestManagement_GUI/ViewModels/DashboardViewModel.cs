using System;
using MalakaBookFestManagement_GUI.Services;

namespace MalakaBookFestManagement_GUI.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private ViewModelBase _currentView = null!;
        private string _activeMenu = "Dashboard";

        public ViewModelBase CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string ActiveMenu
        {
            get => _activeMenu;
            set
            {
                if (SetProperty(ref _activeMenu, value))
                {
                    OnPropertyChanged(nameof(IsDashboardActive));
                    OnPropertyChanged(nameof(IsBooksActive));
                    OnPropertyChanged(nameof(IsTicketsActive));
                    OnPropertyChanged(nameof(IsBoothsActive));
                    OnPropertyChanged(nameof(IsTalkshowsActive));
                    OnPropertyChanged(nameof(IsUsersActive));
                }
            }
        }

        // Helpers to bind active menu items to styles in XAML
        public bool IsDashboardActive => ActiveMenu == "Dashboard";
        public bool IsBooksActive => ActiveMenu == "Books";
        public bool IsTicketsActive => ActiveMenu == "Tickets";
        public bool IsBoothsActive => ActiveMenu == "Booths";
        public bool IsTalkshowsActive => ActiveMenu == "Talkshows";
        public bool IsUsersActive => ActiveMenu == "Users";

        // Roles permissions
        public string UserFullName => ApiService.Instance.CurrentUser?.FullName ?? "User";
        public string UserRole => ApiService.Instance.CurrentUser?.Role ?? "Attendee";
        public string UserEmail => ApiService.Instance.CurrentUser?.Email ?? "";

        public bool IsAdmin => UserRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        public bool IsOrganizer => UserRole.Equals("Organizer", StringComparison.OrdinalIgnoreCase) || IsAdmin;
        public bool IsAttendee => UserRole.Equals("Attendee", StringComparison.OrdinalIgnoreCase) || IsOrganizer;

        public RelayCommand<string> NavigateCommand { get; }
        public RelayCommand LogoutCommand { get; }

        public event Action? OnLogout;

        public DashboardViewModel()
        {
            NavigateCommand = new RelayCommand<string>(ExecuteNavigate);
            LogoutCommand = new RelayCommand(ExecuteLogout);

            // Default view is the Dashboard Overview
            CurrentView = new DashboardOverviewViewModel();
        }

        private void ExecuteNavigate(string? viewName)
        {
            if (string.IsNullOrEmpty(viewName)) return;

            ActiveMenu = viewName;

            switch (viewName)
            {
                case "Dashboard":
                    CurrentView = new DashboardOverviewViewModel();
                    break;
                case "Books":
                    CurrentView = new BooksViewModel();
                    break;
                case "Tickets":
                    CurrentView = new TicketsViewModel();
                    break;
                case "Booths":
                    CurrentView = new BoothsViewModel();
                    break;
                case "Talkshows":
                    CurrentView = new TalkshowsViewModel();
                    break;
                case "Users":
                    if (IsAdmin)
                    {
                        CurrentView = new UsersViewModel();
                    }
                    break;
            }
        }

        private void ExecuteLogout()
        {
            ApiService.Instance.Logout();
            OnLogout?.Invoke();
        }
    }
}
