using BookLib.DTOs;
using BookLib.Entities;
using BookLib.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLib.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookCatalogController : ControllerBase
    {
        private readonly IBookService _bookService;
        
        public BookCatalogController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // IAsyncEnumerable ensures that each incoming book is accessible as it's getting fetched from the db. Task<IEnumerabl> would fetch all the books and then present them.
        [HttpGet]
        public IAsyncEnumerable<Book> GetBooks()
        {
            return _bookService.GetBooks();
        }

        [HttpGet("{title}")]
        public async Task<ActionResult<Book>> GetBookAsync(string title)
        {
            var book = await _bookService.GetBookAsyncOrDefault(title);

            if (book == null)
            {
                return NotFound(title);
            }

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBookAsync(BookCreationDTO bookCreationDTO, CancellationToken cancellationToken)
        {
            var book = await _bookService.CreateBookAsync(bookCreationDTO, cancellationToken);
            return Ok(book);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateBookAsync(int id, BookUpdateDTO bookUpdateDTO, CancellationToken cancellationToken)
        {
            var book = await _bookService.UpdateBookAsyncOrDefault(id, bookUpdateDTO, cancellationToken);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteBookAsync(int id, CancellationToken cancellationToken)
        {
            // cascading delete because ratings should not exist for non existing books
            var book = await _bookService.DeleteBookAsyncOrDefault(id, cancellationToken);
            if (book == null) { return NotFound(); };
            return NoContent();
        }
    }
}
