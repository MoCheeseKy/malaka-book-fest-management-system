using System;
using System.Windows;
using System.Windows.Controls;
using GUI.Models;
using GUI.Services;

namespace GUI.Views
{
    public partial class BoothView : UserControl
    {
        private readonly ApiService _apiService;
        private Booth? _selectedBooth;

        public event Action<Guid>? BoothSelectedForBooks;

        public BoothView()
        {
            InitializeComponent();
            _apiService = new ApiService();
            CmbCategory.ItemsSource = Enum.GetValues(typeof(BoothCategory));
            CmbCategory.SelectedIndex = 0;
            Loaded += BoothView_Loaded;
        }

        private async void BoothView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadBooths();
        }

        private async System.Threading.Tasks.Task LoadBooths()
        {
            try
            {
                var booths = await _apiService.GetBoothsAsync();
                BoothDataGrid.ItemsSource = booths;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading booths: {ex.Message}");
            }
        }

        private void BoothDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Selection logic removed as actions are now in the table rows
        }

        private void AddBooth_Click(object sender, RoutedEventArgs e)
        {
            _selectedBooth = null;
            ModalTitle.Text = "ADD NEW BOOTH";
            ClearForm();
            ModalOverlay.Visibility = Visibility.Visible;
        }

        private void EditBooth_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Booth booth)
            {
                _selectedBooth = booth;
                ModalTitle.Text = "EDIT BOOTH";
                TxtBoothName.Text = _selectedBooth.BoothName;
                TxtBoothNumber.Text = _selectedBooth.BoothNumber;
                TxtDescription.Text = _selectedBooth.Description;
                CmbCategory.SelectedItem = _selectedBooth.Category;
                ModalOverlay.Visibility = Visibility.Visible;
            }
        }

        private void CloseModal_Click(object sender, RoutedEventArgs e)
        {
            ModalOverlay.Visibility = Visibility.Collapsed;
        }

        private void ClearForm()
        {
            TxtBoothName.Text = "";
            TxtBoothNumber.Text = "";
            TxtDescription.Text = "";
            CmbCategory.SelectedIndex = 0;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedBooth == null)
                {
                    // Create
                    var request = new BoothCreateRequest
                    {
                        BoothName = TxtBoothName.Text,
                        BoothNumber = TxtBoothNumber.Text,
                        Description = TxtDescription.Text,
                        Category = (BoothCategory)CmbCategory.SelectedItem
                    };
                    await _apiService.CreateBoothAsync(request);
                    MessageBox.Show("Booth created successfully.");
                }
                else
                {
                    // Update
                    _selectedBooth.BoothName = TxtBoothName.Text;
                    _selectedBooth.BoothNumber = TxtBoothNumber.Text;
                    _selectedBooth.Description = TxtDescription.Text;
                    _selectedBooth.Category = (BoothCategory)CmbCategory.SelectedItem;
                    
                    await _apiService.UpdateBoothAsync(_selectedBooth.BoothId, _selectedBooth);
                    MessageBox.Show("Booth updated successfully.");
                }

                ModalOverlay.Visibility = Visibility.Collapsed;
                ClearForm();
                await LoadBooths();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving booth: {ex.Message}");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Booth booth)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{booth.BoothName}'?", "Confirm Delete", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _apiService.DeleteBoothAsync(booth.BoothId);
                        ClearForm();
                        await LoadBooths();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting booth: {ex.Message}");
                    }
                }
            }
        }

        private void ViewBooksButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Booth booth)
            {
                BoothSelectedForBooks?.Invoke(booth.BoothId);
            }
        }
    }
}
