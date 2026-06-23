using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalakaBookFestManagementSystem_GUI.Services;

namespace MalakaBookFestManagementSystem_GUI.ViewModels
{
    public class TicketPriceItem
    {
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public partial class TicketViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private ObservableCollection<TicketPriceItem> _ticketPrices = new();

        [ObservableProperty]
        private bool _isLoading;

        public TicketViewModel()
        {
            _apiClient = new ApiClient();
            LoadTicketPricesCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadTicketPricesAsync()
        {
            IsLoading = true;
            TicketPrices.Clear();
            
            var data = await _apiClient.GetTicketPricesAsync();
            foreach (var kvp in data)
            {
                TicketPrices.Add(new TicketPriceItem { Type = kvp.Key, Price = kvp.Value });
            }
            
            IsLoading = false;
        }
    }
}
