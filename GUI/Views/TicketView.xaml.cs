using System;
using System.Windows;
using System.Windows.Controls;
using GUI.Services;

namespace GUI.Views
{
    public partial class TicketView : UserControl
    {
        private readonly ApiService _apiService;

        public TicketView()
        {
            InitializeComponent();
            _apiService = new ApiService();
            Loaded += TicketView_Loaded;
        }

        private async void TicketView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTicketPrices();
        }

        private async System.Threading.Tasks.Task LoadTicketPrices()
        {
            try
            {
                var prices = await _apiService.GetTicketPricesAsync();
                TicketPricesList.ItemsSource = prices;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading ticket prices: {ex.Message}");
            }
        }

        private async void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            var qrCode = TxtQrCode.Text;
            if (string.IsNullOrWhiteSpace(qrCode))
            {
                MessageBox.Show("Please enter a QR code to scan.");
                return;
            }

            try
            {
                var ticket = await _apiService.ScanTicketAsync(qrCode);
                
                ScanResultPanel.Visibility = Visibility.Visible;
                TxtResultStatus.Text = "VALID TICKET";
                TxtResultStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LimeGreen);
                
                if (ticket != null)
                {
                    TxtResultType.Text = ticket.Type.ToString();
                    TxtResultDate.Text = ticket.ValidDate.ToString("yyyy-MM-dd");
                    TxtResultPrice.Text = ticket.PricePaid.ToString("C");
                }
                else
                {
                    TxtResultType.Text = "Standard";
                    TxtResultDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    TxtResultPrice.Text = "Verified";
                }

                TxtQrCode.Text = "";
            }
            catch (Exception ex)
            {
                ScanResultPanel.Visibility = Visibility.Visible;
                TxtResultStatus.Text = "INVALID TICKET";
                TxtResultStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                TxtResultType.Text = "-";
                TxtResultDate.Text = "-";
                TxtResultPrice.Text = "-";
                
                MessageBox.Show($"Error scanning ticket: {ex.Message}");
            }
        }
    }
}
