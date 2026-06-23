using System;
using System.Linq;
using System.Threading.Tasks;
using MalakaBookFestManagement_GUI.Services;

namespace MalakaBookFestManagement_GUI.ViewModels
{
    public class DashboardOverviewViewModel : ViewModelBase
    {
        private int _totalBooths;
        private int _totalBooks;
        private int _totalTalkshows;
        private int _totalTickets;
        private bool _isLoading;

        public int TotalBooths
        {
            get => _totalBooths;
            set => SetProperty(ref _totalBooths, value);
        }

        public int TotalBooks
        {
            get => _totalBooks;
            set => SetProperty(ref _totalBooks, value);
        }

        public int TotalTalkshows
        {
            get => _totalTalkshows;
            set => SetProperty(ref _totalTalkshows, value);
        }

        public int TotalTickets
        {
            get => _totalTickets;
            set => SetProperty(ref _totalTickets, value);
        }

        public string UserFullName => ApiService.Instance.CurrentUser?.FullName ?? "User";
        public string UserRole => ApiService.Instance.CurrentUser?.Role ?? "Attendee";

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public RelayCommand LoadStatsCommand { get; }

        public DashboardOverviewViewModel()
        {
            LoadStatsCommand = new RelayCommand(async () => await LoadStatsAsync());
            _ = LoadStatsAsync(); // Load on construct
        }

        public async Task LoadStatsAsync()
        {
            IsLoading = true;
            try
            {
                // Fetch booths
                var boothsResponse = await ApiService.Instance.GetBoothsAsync();
                if (boothsResponse.Success && boothsResponse.Data != null)
                {
                    TotalBooths = boothsResponse.Data.Count();

                    // Calculate total books by fetching for each booth (limit to active or first few if many, or fetch all)
                    int bookCount = 0;
                    foreach (var booth in boothsResponse.Data)
                    {
                        var booksResponse = await ApiService.Instance.GetBooksByBoothAsync(booth.BoothId);
                        if (booksResponse.Success && booksResponse.Data != null)
                        {
                            bookCount += booksResponse.Data.Count();
                        }
                    }
                    TotalBooks = bookCount;
                }

                // Fetch talkshows
                var talkshowsResponse = await ApiService.Instance.GetTalkshowsAsync();
                if (talkshowsResponse.Success && talkshowsResponse.Data != null)
                {
                    TotalTalkshows = talkshowsResponse.Data.Count();
                }

                // Fetch tickets
                if (UserRole == "Admin")
                {
                    // Admin can see total registered users or tickets, since tickets are private, let's fetch my tickets or count users
                    var usersResponse = await ApiService.Instance.GetUsersAsync();
                    if (usersResponse.Success && usersResponse.Data != null)
                    {
                        TotalTickets = usersResponse.Data.Count(); // Displays user count as indicator
                    }
                }
                else
                {
                    // Attendee sees their own ticket count
                    var ticketsResponse = await ApiService.Instance.GetMyTicketsAsync();
                    if (ticketsResponse.Success && ticketsResponse.Data != null)
                    {
                        TotalTickets = ticketsResponse.Data.Count();
                    }
                }
            }
            catch (Exception)
            {
                // Gracefully fail stats loading
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
