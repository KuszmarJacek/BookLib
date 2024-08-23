using BookLib.DTOs;
using BookLib.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookLib.Data
{
    public interface IBookRepository
    {
        public IAsyncEnumerable<Book> GetBooks();
        public Task<Book> GetBookAsyncOrDefault(string title);
        public Task<Book> CreateBookAsync(BookCreationDTO bookCreationDTO, CancellationToken cancellationToken);
        public Task<Book> UpdateBookAsyncOrDefault(int id, BookUpdateDTO bookUpdateDTO, CancellationToken cancellationToken);
        public Task<Book> DeleteBookAsyncOrDefault(int id, CancellationToken cancellationToken);
    }
}
