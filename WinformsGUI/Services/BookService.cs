using WinformsGUI.Models;
using WinformsGUI.Utils;

namespace WinformsGUI.Services
{
    public class BookService
    {
        public async Task<List<BookResponse>?> GetBooksByBoothAsync(Guid boothId)
        {
            return await ApiClient.GetAsync<List<BookResponse>>($"booths/{boothId}/books");
        }

        public async Task<BookResponse?> CreateBookAsync(Guid boothId, CreateBookRequest request)
        {
            return await ApiClient.PostAsync<CreateBookRequest, BookResponse>($"booths/{boothId}/books", request);
        }

        public async Task<BookResponse?> UpdateBookAsync(Guid boothId, Guid bookId, UpdateBookRequest request)
        {
            return await ApiClient.PutAsync<UpdateBookRequest, BookResponse>($"booths/{boothId}/books/{bookId}", request);
        }

        public async Task DeleteBookAsync(Guid boothId, Guid bookId)
        {
            await ApiClient.DeleteAsync($"booths/{boothId}/books/{bookId}");
        }
    }
}
