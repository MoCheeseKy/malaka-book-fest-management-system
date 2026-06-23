using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MalakaBookFestManagement_GUI.Services;

namespace MalakaBookFestManagement_GUI.ViewModels
{
    public class TicketsViewModel : ViewModelBase
    {
        private ObservableCollection<TicketDto> _tickets = new();
        private TicketDto? _selectedTicket;

        // Purchase Bindings
        private string _selectedType = "SingleDay";
        private DateTime _validDate = DateTime.Today;
        private List<string> _ticketTypes = new() { "SingleDay", "AllAccess", "VIP" };

        // Scan Bindings (Admin only)
        private string _qrCodeInput = string.Empty;
        private string _scanResult = string.Empty;
        private TicketDto? _scannedTicket;

        private bool _isLoading;
        private string _statusMessage = string.Empty;
        private bool _isSuccessMessage;

        public ObservableCollection<TicketDto> Tickets
        {
            get => _tickets;
            set => SetProperty(ref _tickets, value);
        }

        public TicketDto? SelectedTicket
        {
            get => _selectedTicket;
            set => SetProperty(ref _selectedTicket, value);
        }

        // Purchase Bindings
        public string SelectedType
        {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value);
        }

        public DateTime ValidDate
        {
            get => _validDate;
            set => SetProperty(ref _validDate, value);
        }

        public List<string> TicketTypes
        {
            get => _ticketTypes;
            set => SetProperty(ref _ticketTypes, value);
        }

        // Scan Bindings
        public string QrCodeInput
        {
            get => _qrCodeInput;
            set => SetProperty(ref _qrCodeInput, value);
        }

        public string ScanResult
        {
            get => _scanResult;
            set => SetProperty(ref _scanResult, value);
        }

        public TicketDto? ScannedTicket
        {
            get => _scannedTicket;
            set => SetProperty(ref _scannedTicket, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsSuccessMessage
        {
            get => _isSuccessMessage;
            set => SetProperty(ref _isSuccessMessage, value);
        }

        public bool IsAdmin => ApiService.Instance.CurrentUser?.Role == "Admin";

        public RelayCommand PurchaseCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ScanCommand { get; }
        public RelayCommand LoadTicketsCommand { get; }

        public TicketsViewModel()
        {
            PurchaseCommand = new RelayCommand(async () => await PurchaseTicketAsync());
            CancelCommand = new RelayCommand(async () => await CancelTicketAsync(), () => SelectedTicket != null);
            ScanCommand = new RelayCommand(async () => await ScanTicketAsync(), () => IsAdmin && !string.IsNullOrWhiteSpace(QrCodeInput));
            LoadTicketsCommand = new RelayCommand(async () => await LoadTicketsAsync());

            _ = LoadTicketsAsync();
        }

        private async Task LoadTicketsAsync()
        {
            IsLoading = true;
            try
            {
                var response = await ApiService.Instance.GetMyTicketsAsync();
                if (response.Success && response.Data != null)
                {
                    Tickets = new ObservableCollection<TicketDto>(response.Data);
                }
                else
                {
                    ShowError(response.Message ?? "Failed to load tickets.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading tickets: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task PurchaseTicketAsync()
        {
            IsLoading = true;
            try
            {
                var dto = new PurchaseTicketDto
                {
                    Type = SelectedType,
                    ValidDate = ValidDate
                };

                var response = await ApiService.Instance.PurchaseTicketAsync(dto);
                if (response.Success && response.Data != null)
                {
                    ShowSuccess($"Ticket purchased successfully! Code: {response.Data.QrCode}");
                    await LoadTicketsAsync();
                }
                else
                {
                    ShowError(response.Message ?? "Failed to purchase ticket.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error purchasing ticket: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CancelTicketAsync()
        {
            if (SelectedTicket == null) return;

            if (System.Windows.MessageBox.Show($"Are you sure you want to cancel this ticket ({SelectedTicket.Type})?", "Confirm Cancellation", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) != System.Windows.MessageBoxResult.Yes)
            {
                return;
            }

            IsLoading = true;
            try
            {
                var response = await ApiService.Instance.CancelTicketAsync(SelectedTicket.TicketId);
                if (response.Success)
                {
                    ShowSuccess("Ticket cancelled successfully.");
                    await LoadTicketsAsync();
                }
                else
                {
                    ShowError(response.Message ?? "Failed to cancel ticket.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error cancelling ticket: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ScanTicketAsync()
        {
            if (string.IsNullOrWhiteSpace(QrCodeInput)) return;

            IsLoading = true;
            ScanResult = string.Empty;
            ScannedTicket = null;

            try
            {
                var response = await ApiService.Instance.ScanTicketAsync(QrCodeInput);
                if (response.Success && response.Data != null)
                {
                    ScannedTicket = response.Data;
                    ScanResult = "SUCCESS: Ticket scanned and verified successfully.";
                    QrCodeInput = string.Empty; // clear input
                    await LoadTicketsAsync(); // reload to reflect status if my tickets
                }
                else
                {
                    ScanResult = $"FAILED: {response.Message ?? "Scan rejected."}";
                }
            }
            catch (Exception ex)
            {
                ScanResult = $"ERROR: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ShowError(string message)
        {
            StatusMessage = message;
            IsSuccessMessage = false;
        }

        private void ShowSuccess(string message)
        {
            StatusMessage = message;
            IsSuccessMessage = true;
        }
    }
}
