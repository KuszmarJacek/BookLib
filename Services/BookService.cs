using BookLib.Data;
using BookLib.DTOs;
using BookLib.Entities;

namespace BookLib.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public IAsyncEnumerable<Book> GetBooks()
        {
            return _bookRepository.GetBooks();
        }
        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public Task<Book> GetBookAsyncOrDefault(string title)
        {
            return _bookRepository.GetBookAsyncOrDefault(title);
        }
        public Task<Book> CreateBookAsync(BookCreationDTO bookCreationDTO, CancellationToken cancellationToken)
        {
            return _bookRepository.CreateBookAsync(bookCreationDTO, cancellationToken);
        }
        public Task<Book> UpdateBookAsyncOrDefault(int id, BookUpdateDTO bookUpdateDTO, CancellationToken cancellationToken)
        {
            return _bookRepository.UpdateBookAsyncOrDefault(id, bookUpdateDTO, cancellationToken);
        }
        public Task<Book> DeleteBookAsyncOrDefault(int id, CancellationToken cancellationToken)
        {
            return _bookRepository.DeleteBookAsyncOrDefault(id, cancellationToken);
        }
    }
}
