using System;
using System.Windows;
using System.Windows.Controls;
using GUI.Models;
using GUI.Services;

namespace GUI.Views
{
    public partial class BookView : UserControl
    {
        private readonly ApiService _apiService;
        private Guid _boothId;
        private Book? _selectedBook;

        public BookView()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        public async void LoadBooksForBooth(Guid boothId)
        {
            _boothId = boothId;
            TxtCurrentBoothId.Text = $"Booth ID: {boothId}";
            await RefreshBooks();
        }

        private async System.Threading.Tasks.Task RefreshBooks()
        {
            if (_boothId == Guid.Empty) return;

            try
            {
                var books = await _apiService.GetBooksByBoothAsync(_boothId);
                BookDataGrid.ItemsSource = books;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading books: {ex.Message}");
            }
        }

        private void BookDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Selection logic removed as actions are now in the table rows
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            if (_boothId == Guid.Empty)
            {
                MessageBox.Show("Please select a Booth first!");
                return;
            }
            
            _selectedBook = null;
            ModalTitle.Text = "ADD NEW BOOK";
            ClearForm();
            ModalOverlay.Visibility = Visibility.Visible;
        }

        private void EditBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Book book)
            {
                _selectedBook = book;
                ModalTitle.Text = "EDIT BOOK";
                TxtTitle.Text = _selectedBook.Title;
                TxtAuthor.Text = _selectedBook.Author;
                TxtIsbn.Text = _selectedBook.Isbn;
                TxtPrice.Text = _selectedBook.Price.ToString();
                TxtStock.Text = _selectedBook.Stock.ToString();
                TxtCoverUrl.Text = _selectedBook.CoverUrl;
                
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
            TxtAuthor.Text = "";
            TxtIsbn.Text = "";
            TxtPrice.Text = "0";
            TxtStock.Text = "0";
            TxtCoverUrl.Text = "";
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!decimal.TryParse(TxtPrice.Text, out var price)) price = 0;
                if (!int.TryParse(TxtStock.Text, out var stock)) stock = 0;

                if (_selectedBook == null)
                {
                    // Create
                    var req = new BookCreateRequest
                    {
                        Title = TxtTitle.Text,
                        Author = TxtAuthor.Text,
                        Isbn = TxtIsbn.Text,
                        Price = price,
                        Stock = stock,
                        CoverUrl = TxtCoverUrl.Text
                    };
                    await _apiService.CreateBookAsync(_boothId, req);
                    MessageBox.Show("Book added successfully.");
                }
                else
                {
                    // Update
                    _selectedBook.Title = TxtTitle.Text;
                    _selectedBook.Author = TxtAuthor.Text;
                    _selectedBook.Isbn = TxtIsbn.Text;
                    _selectedBook.Price = price;
                    _selectedBook.Stock = stock;
                    _selectedBook.CoverUrl = TxtCoverUrl.Text;

                    await _apiService.UpdateBookAsync(_boothId, _selectedBook.BookId, _selectedBook);
                    MessageBox.Show("Book updated successfully.");
                }

                ModalOverlay.Visibility = Visibility.Collapsed;
                ClearForm();
                await RefreshBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving book: {ex.Message}");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Book book)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{book.Title}'?", "Confirm Delete", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _apiService.DeleteBookAsync(_boothId, book.BookId);
                        ClearForm();
                        await RefreshBooks();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting book: {ex.Message}");
                    }
                }
            }
        }
    }
}
