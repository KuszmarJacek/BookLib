using BookLib.Config;
using BookLib.DTOs;
using BookLib.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLib.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookCatalogController : ControllerBase
    {
        private readonly BookCatalogContext _dbContext;

        public BookCatalogController(BookCatalogContext dbContext)
        {
            _dbContext = dbContext;
        }

        // IAsyncEnumerable ensures that each incoming book is accessible as it's getting fetched from the db. Task<IEnumerabl> would fetch all the books and then present them.
        [HttpGet]
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

        [HttpGet("{title}")]
        public async Task<ActionResult<Book>> GetBook(string title)
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
                return NotFound(title);
            }

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(BookCreationDTO bookCreationDTO, CancellationToken cancellationToken)
        {
            var book = new Book { Title = bookCreationDTO.Title, Description = bookCreationDTO.Description };
            var entityEntry = _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Ok(entityEntry.Entity);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateBook(int id, BookUpdateDTO bookUpdateDTO, CancellationToken cancellationToken)
        {
            var book = await _dbContext.FindAsync<Book>([id], cancellationToken);
            if (book == null) { return NotFound(); }
            if (bookUpdateDTO.Title != null)
            {
                book.Title = bookUpdateDTO.Title;
            }
            if (bookUpdateDTO.Description != null)
            {
                book.Description = bookUpdateDTO.Description;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Ok(book);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteBook(int id, CancellationToken cancellationToken)
        {
            // cascading delete because ratings should not exist for non existing books
            var book = await _dbContext.Books.Include(book => book.Ratings).FirstOrDefaultAsync(book => book.Id == id, cancellationToken);
            if (book == null) { return NotFound(); };
            _dbContext.Remove(book);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
    }
}
