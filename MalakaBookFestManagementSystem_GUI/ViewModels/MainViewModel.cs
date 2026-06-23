using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MalakaBookFestManagementSystem_GUI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject? _currentViewModel;

        public MainViewModel()
        {
            // Set default view
            CurrentViewModel = new BoothViewModel();
        }

        [RelayCommand]
        private void NavigateToBooth()
        {
            CurrentViewModel = new BoothViewModel();
        }

        [RelayCommand]
        private void NavigateToBook()
        {
            CurrentViewModel = new BookViewModel();
        }

        [RelayCommand]
        private void NavigateToTalkshow()
        {
            CurrentViewModel = new TalkshowViewModel();
        }

        [RelayCommand]
        private void NavigateToTicket()
        {
            CurrentViewModel = new TicketViewModel();
        }
    }
}
