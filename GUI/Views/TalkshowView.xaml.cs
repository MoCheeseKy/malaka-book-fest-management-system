using System;
using System.Windows;
using System.Windows.Controls;
using GUI.Models;
using GUI.Services;

namespace GUI.Views
{
    public partial class TalkshowView : UserControl
    {
        private readonly ApiService _apiService;
        private Talkshow? _selectedTalkshow;

        public TalkshowView()
        {
            InitializeComponent();
            _apiService = new ApiService();
            Loaded += TalkshowView_Loaded;
        }

        private async void TalkshowView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTalkshows();
        }

        private async System.Threading.Tasks.Task LoadTalkshows()
        {
            try
            {
                var talkshows = await _apiService.GetTalkshowsAsync();
                TalkshowDataGrid.ItemsSource = talkshows;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading talkshows: {ex.Message}");
            }
        }

        private void TalkshowDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Selection logic removed as actions are now in the table rows
        }

        private void AddTalkshow_Click(object sender, RoutedEventArgs e)
        {
            _selectedTalkshow = null;
            ModalTitle.Text = "ADD NEW TALKSHOW";
            ClearForm();
            ModalOverlay.Visibility = Visibility.Visible;
        }

        private void EditTalkshow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Talkshow ts)
            {
                _selectedTalkshow = ts;
                ModalTitle.Text = "EDIT TALKSHOW";
                TxtTitle.Text = _selectedTalkshow.Title;
                TxtSpeakerName.Text = _selectedTalkshow.SpeakerName;
                TxtSpeakerBio.Text = _selectedTalkshow.SpeakerBio;
                TxtVenue.Text = _selectedTalkshow.Venue;
                TxtStartTime.Text = _selectedTalkshow.StartTime.ToString("yyyy-MM-dd HH:mm");
                TxtEndTime.Text = _selectedTalkshow.EndTime.ToString("yyyy-MM-dd HH:mm");
                TxtMaxCapacity.Text = _selectedTalkshow.MaxCapacity.ToString();
                
                ModalOverlay.Visibility = Visibility.Visible;
            }
        }

        private void CloseModal_Click(object sender, RoutedEventArgs e)
        {
            ModalOverlay.Visibility = Visibility.Collapsed;
        }

        private void ClearForm()
        {
            TxtTitle.Text = "";
            TxtSpeakerName.Text = "";
            TxtSpeakerBio.Text = "";
            TxtVenue.Text = "";
            TxtStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            TxtEndTime.Text = DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm");
            TxtMaxCapacity.Text = "100";
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!DateTime.TryParse(TxtStartTime.Text, out var start)) start = DateTime.Now;
                if (!DateTime.TryParse(TxtEndTime.Text, out var end)) end = DateTime.Now.AddHours(2);
                if (!int.TryParse(TxtMaxCapacity.Text, out var cap)) cap = 100;

                if (_selectedTalkshow == null)
                {
                    // Create
                    var req = new TalkshowCreateRequest
                    {
                        Title = TxtTitle.Text,
                        SpeakerName = TxtSpeakerName.Text,
                        SpeakerBio = TxtSpeakerBio.Text,
                        Venue = TxtVenue.Text,
                        StartTime = start,
                        EndTime = end,
                        MaxCapacity = cap
                    };
                    await _apiService.CreateTalkshowAsync(req);
                    MessageBox.Show("Talkshow created successfully.");
                }
                else
                {
                    // Update
                    _selectedTalkshow.Title = TxtTitle.Text;
                    _selectedTalkshow.SpeakerName = TxtSpeakerName.Text;
                    _selectedTalkshow.SpeakerBio = TxtSpeakerBio.Text;
                    _selectedTalkshow.Venue = TxtVenue.Text;
                    _selectedTalkshow.StartTime = start;
                    _selectedTalkshow.EndTime = end;
                    _selectedTalkshow.MaxCapacity = cap;

                    await _apiService.UpdateTalkshowAsync(_selectedTalkshow.TalkshowId, _selectedTalkshow);
                    MessageBox.Show("Talkshow updated successfully.");
                }

                ModalOverlay.Visibility = Visibility.Collapsed;
                ClearForm();
                await LoadTalkshows();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving talkshow: {ex.Message}");
            }
        }

        private async void AdvanceStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Talkshow ts)
            {
                var result = MessageBox.Show($"Are you sure you want to advance the status of '{ts.Title}'?", "Confirm Status Change", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _apiService.AdvanceTalkshowStatusAsync(ts.TalkshowId);
                        MessageBox.Show("Status advanced successfully!");
                        await LoadTalkshows();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error advancing status: {ex.Message}");
                    }
                }
            }
        }
    }
}
