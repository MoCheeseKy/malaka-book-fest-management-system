using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MalakaBookFestManagement_GUI.Services;

namespace MalakaBookFestManagement_GUI.ViewModels
{
    public class BooksViewModel : ViewModelBase
    {
        private ObservableCollection<BoothDto> _booths = new();
        private BoothDto? _selectedBooth;
        private ObservableCollection<BookDto> _books = new();
        private BookDto? _selectedBook;

        // Form Fields
        private string _title = string.Empty;
        private string _author = string.Empty;
        private string _isbn = string.Empty;
        private decimal _price;
        private int _stock;
        private string _coverUrl = string.Empty;

        private bool _isLoading;
        private string _statusMessage = string.Empty;
        private bool _isSuccessMessage;
        private bool _isEditMode;

        public ObservableCollection<BoothDto> Booths
        {
            get => _booths;
            set => SetProperty(ref _booths, value);
        }

        public BoothDto? SelectedBooth
        {
            get => _selectedBooth;
            set
            {
                if (SetProperty(ref _selectedBooth, value))
                {
                    ResetForm();
                    _ = LoadBooksAsync();
                }
            }
        }

        public ObservableCollection<BookDto> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        public BookDto? SelectedBook
        {
            get => _selectedBook;
            set
            {
                if (SetProperty(ref _selectedBook, value))
                {
                    if (value != null)
                    {
                        Title = value.Title;
                        Author = value.Author;
                        Isbn = value.Isbn;
                        Price = value.Price;
                        Stock = value.Stock;
                        CoverUrl = value.CoverUrl;
                        IsEditMode = true;
                    }
                    else
                    {
                        ResetForm();
                    }
                }
            }
        }

        // Form Bindings
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        public string Isbn
        {
            get => _isbn;
            set => SetProperty(ref _isbn, value);
        }

        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public int Stock
        {
            get => _stock;
            set => SetProperty(ref _stock, value);
        }

        public string CoverUrl
        {
            get => _coverUrl;
            set => SetProperty(ref _coverUrl, value);
        }

        // UI States
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

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        // Role authorization check
        public bool CanEdit => ApiService.Instance.CurrentUser?.Role == "Admin" || ApiService.Instance.CurrentUser?.Role == "Organizer";

        public RelayCommand SaveCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ClearFormCommand { get; }

        public BooksViewModel()
        {
            SaveCommand = new RelayCommand(async () => await SaveBookAsync(), () => CanEdit && SelectedBooth != null && !string.IsNullOrWhiteSpace(Title));
            DeleteCommand = new RelayCommand(async () => await DeleteBookAsync(), () => CanEdit && SelectedBook != null);
            ClearFormCommand = new RelayCommand(ResetForm);

            _ = LoadBoothsAsync();
        }

        private async Task LoadBoothsAsync()
        {
            IsLoading = true;
            try
            {
                var response = await ApiService.Instance.GetBoothsAsync();
                if (response.Success && response.Data != null)
                {
                    Booths = new ObservableCollection<BoothDto>(response.Data.Where(b => b.IsActive));
                    if (Booths.Any())
                    {
                        SelectedBooth = Booths.First();
                    }
                }
                else
                {
                    ShowError(response.Message ?? "Failed to load booths.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading booths: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadBooksAsync()
        {
            if (SelectedBooth == null)
            {
                Books.Clear();
                return;
            }

            IsLoading = true;
            try
            {
                var response = await ApiService.Instance.GetBooksByBoothAsync(SelectedBooth.BoothId);
                if (response.Success && response.Data != null)
                {
                    Books = new ObservableCollection<BookDto>(response.Data);
                }
                else
                {
                    ShowError(response.Message ?? "Failed to load books.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading books: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SaveBookAsync()
        {
            if (SelectedBooth == null) return;

            IsLoading = true;
            try
            {
                if (IsEditMode && SelectedBook != null)
                {
                    var dto = new UpdateBookDto
                    {
                        Title = Title,
                        Author = Author,
                        Isbn = Isbn,
                        Price = Price,
                        Stock = Stock,
                        CoverUrl = CoverUrl
                    };

                    var response = await ApiService.Instance.UpdateBookAsync(SelectedBooth.BoothId, SelectedBook.BookId, dto);
                    if (response.Success)
                    {
                        ShowSuccess("Book updated successfully.");
                        ResetForm();
                        await LoadBooksAsync();
                    }
                    else
                    {
                        ShowError(response.Message ?? "Failed to update book.");
                    }
                }
                else
                {
                    var dto = new CreateBookDto
                    {
                        Title = Title,
                        Author = Author,
                        Isbn = Isbn,
                        Price = Price,
                        Stock = Stock,
                        CoverUrl = CoverUrl
                    };

                    var response = await ApiService.Instance.AddBookAsync(SelectedBooth.BoothId, dto);
                    if (response.Success)
                    {
                        ShowSuccess("Book added successfully.");
                        ResetForm();
                        await LoadBooksAsync();
                    }
                    else
                    {
                        ShowError(response.Message ?? "Failed to add book.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error saving book: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteBookAsync()
        {
            if (SelectedBooth == null || SelectedBook == null) return;

            if (System.Windows.MessageBox.Show($"Are you sure you want to delete '{SelectedBook.Title}'?", "Confirm Delete", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning) != System.Windows.MessageBoxResult.Yes)
            {
                return;
            }

            IsLoading = true;
            try
            {
                var response = await ApiService.Instance.DeleteBookAsync(SelectedBooth.BoothId, SelectedBook.BookId);
                if (response.Success)
                {
                    ShowSuccess("Book deleted successfully.");
                    ResetForm();
                    await LoadBooksAsync();
                }
                else
                {
                    ShowError(response.Message ?? "Failed to delete book.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error deleting book: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ResetForm()
        {
            Title = string.Empty;
            Author = string.Empty;
            Isbn = string.Empty;
            Price = 0;
            Stock = 0;
            CoverUrl = string.Empty;
            IsEditMode = false;
            _selectedBook = null;
            OnPropertyChanged(nameof(SelectedBook));
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
