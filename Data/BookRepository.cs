using BookLib.DTOs;
using BookLib.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLib.Data
{
    public class BookRepository : IBookRepository
    {
        private readonly RepositoryContext _dbContext;

        public BookRepository(RepositoryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Book> DeleteBookAsyncOrDefault(int id, CancellationToken cancellationToken)
        {
            // cascading delete because ratings should not exist for non existing books
            var book = await _dbContext.Books.Include(book => book.Ratings).FirstOrDefaultAsync(book => book.Id == id, cancellationToken);
            if (book == null) { return null; };
            _dbContext.Remove(book);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return book;
        }

        public async Task<Book> UpdateBookAsyncOrDefault(int id, BookUpdateDTO bookUpdateDTO, CancellationToken cancellationToken)
        {
            var book = await _dbContext.FindAsync<Book>([id], cancellationToken);
            if (book == null) { return null; }
            if (bookUpdateDTO.Title != null)
            {
                book.Title = bookUpdateDTO.Title;
            }
            if (bookUpdateDTO.Description != null)
            {
                book.Description = bookUpdateDTO.Description;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return book;
        }

        public async Task<Book> CreateBookAsync(BookCreationDTO bookCreationDTO, CancellationToken cancellationToken)
        {
            var book = new Book { Title = bookCreationDTO.Title, Description = bookCreationDTO.Description };
            var entityEntry = _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return book;
        }

        public async Task<Book> GetBookAsyncOrDefault(string title)
        {
            IQueryable<Book> query = _dbContext.Books
                .Where(book => book.Title.ToLower()
                .Equals(title.ToLower()))
                .Include(book => book.Ratings)
                .Include(book => book.Authors)
                .Include(book => book.Tags)
                .AsNoTracking();
            var book = await query.FirstOrDefaultAsync();

            if (book == null)
            {
                return null;
            }

            return book;
        }

        public IAsyncEnumerable<Book> GetBooks()
        {
            // AsNoTracking to prevent reading values that are overwritten at the moment
            IQueryable<Book> query = _dbContext.Books
                .Include(book => book.Ratings)
                .Include(book => book.Authors)
                .Include(book => book.Tags)
                .AsNoTracking();
            return query.AsAsyncEnumerable();
        }
    }
}
