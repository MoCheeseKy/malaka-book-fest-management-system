using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalakaBookFestManagementSystem_GUI.Models;
using MalakaBookFestManagementSystem_GUI.Services;

namespace MalakaBookFestManagementSystem_GUI.ViewModels
{
    public partial class BookViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private ObservableCollection<BoothDto> _booths = new();

        private BoothDto? _selectedBooth;
        public BoothDto? SelectedBooth
        {
            get => _selectedBooth;
            set
            {
                if (SetProperty(ref _selectedBooth, value))
                {
                    if (_selectedBooth != null)
                    {
                        LoadBooksCommand.Execute(null);
                    }
                    else
                    {
                        Books.Clear();
                    }
                }
            }
        }

        [ObservableProperty]
        private ObservableCollection<BookDto> _books = new();

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isSidePanelOpen;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private bool _isDetailMode;

        [ObservableProperty]
        private bool _isNotDetailMode = true;

        [ObservableProperty]
        private string _sidePanelTitle = "ADD BOOK";

        [ObservableProperty]
        private BookDto _currentBook = new();

        public BookViewModel()
        {
            _apiClient = new ApiClient();
            LoadBoothsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadBoothsAsync()
        {
            IsLoading = true;
            Booths.Clear();
            var data = await _apiClient.GetBoothsAsync();
            foreach (var item in data) Booths.Add(item);
            IsLoading = false;
        }

        [RelayCommand]
        private async Task LoadBooksAsync()
        {
            if (SelectedBooth == null) return;
            IsLoading = true;
            Books.Clear();
            var data = await _apiClient.GetBooksByBoothAsync(SelectedBooth.BoothId);
            foreach (var item in data) Books.Add(item);
            IsLoading = false;
        }

        [RelayCommand]
        private void PrepareAdd()
        {
            if (SelectedBooth == null) return; // Must select booth first
            CurrentBook = new BookDto { BoothId = SelectedBooth.BoothId };
            IsEditMode = false;
            IsDetailMode = false;
            IsNotDetailMode = true;
            SidePanelTitle = "ADD BOOK";
            IsSidePanelOpen = true;
        }

        [RelayCommand]
        private void PrepareEdit(BookDto book)
        {
            if (book == null) return;
            CurrentBook = new BookDto
            {
                BookId = book.BookId,
                BoothId = book.BoothId,
                Title = book.Title,
                Author = book.Author,
                Isbn = book.Isbn,
                Price = book.Price,
                Stock = book.Stock
            };
            IsEditMode = true;
            IsDetailMode = false;
            IsNotDetailMode = true;
            SidePanelTitle = "EDIT BOOK";
            IsSidePanelOpen = true;
        }

        [RelayCommand]
        private void ShowDetails(BookDto book)
        {
            if (book == null) return;
            CurrentBook = book;
            IsEditMode = false;
            IsDetailMode = true;
            IsNotDetailMode = false;
            SidePanelTitle = "BOOK DETAILS";
            IsSidePanelOpen = true;
        }

        [RelayCommand]
        private void CloseSidePanel()
        {
            IsSidePanelOpen = false;
        }

        [RelayCommand]
        private async Task SaveBookAsync()
        {
            if (SelectedBooth == null) return;
            IsLoading = true;
            bool success = false;

            if (IsEditMode)
            {
                success = await _apiClient.UpdateBookAsync(SelectedBooth.BoothId, CurrentBook.BookId, CurrentBook);
            }
            else
            {
                success = await _apiClient.CreateBookAsync(SelectedBooth.BoothId, CurrentBook);
            }
            IsLoading = false;

            if (success)
            {
                IsSidePanelOpen = false;
                await LoadBooksAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteBookAsync(BookDto book)
        {
            if (SelectedBooth == null || book == null) return;
            IsLoading = true;
            var success = await _apiClient.DeleteBookAsync(SelectedBooth.BoothId, book.BookId);
            IsLoading = false;

            if (success)
            {
                await LoadBooksAsync();
            }
        }
    }
}
