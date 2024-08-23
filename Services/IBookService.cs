using BookLib.DTOs;
using BookLib.Entities;

namespace BookLib.Services
{
    public interface IBookService
    {
        public IAsyncEnumerable<Book> GetBooks();
        public Task<Book> GetBookAsyncOrDefault(string title);
        public Task<Book> CreateBookAsync(BookCreationDTO bookCreationDTO, CancellationToken cancellationToken);
        public Task<Book> UpdateBookAsyncOrDefault(int id, BookUpdateDTO bookUpdateDTO, CancellationToken cancellationToken);
        public Task<Book> DeleteBookAsyncOrDefault(int id, CancellationToken cancellationToken);
    }
}
