using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LibraryApp.Data;
using LibraryApp.Models;
namespace LibraryApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        public BooksController(ApplicationDbContext context)
        {
            Context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetBooks()
        {
            return await Context.Books.Include(b => b.Reviews).ToListAsync();
        }

        [HttpGet("featured")]
        public async Task<ActionResult<List<Book>>> GetFeaturedBooks([FromQuery] int count = 5)
        {
            count = Math.Clamp(count, 1, 20);

            return await Context.Books
                .Include(b => b.Reviews)
                .OrderBy(b => Guid.NewGuid())
                .Take(count)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await Context.Books.Include(b => b.Reviews).FirstOrDefaultAsync(b => b.Id == id);
            if (book != null)
            {
                return book;
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<Book>> CreateBook([FromBody] Book book)
        {
            Context.Books.Add(book);
            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<Book>> UpdateBook(int id, [FromBody] Book book)
        {
            if (id == book.Id)
            {
                var existingBook = await Context.Books.FindAsync(id);
                if (existingBook != null)
                {
                    // IsAvailable is intentionally left untouched here - it's only meant to change
                    // via the checkout/return endpoints in BookingsController, not a general edit.
                    existingBook.Title = book.Title;
                    existingBook.Author = book.Author;
                    existingBook.Description = book.Description;
                    existingBook.CoverImage = book.CoverImage;
                    existingBook.Publisher = book.Publisher;
                    existingBook.PublicationDate = book.PublicationDate;
                    existingBook.Category = book.Category;
                    existingBook.ISBN = book.ISBN;
                    existingBook.PageCount = book.PageCount;

                    await Context.SaveChangesAsync();

                    return existingBook;
                }

                return NotFound();
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await Context.Books.FindAsync(id);
            if (book != null)
            {
                Context.Books.Remove(book);
                await Context.SaveChangesAsync();

                return NoContent();
            }

            return NotFound();
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Book>>> SearchBooks([FromQuery] string query)
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                // Reviews are included so the client can derive averageRating, same as GetBooks.
                var books = await Context.Books
                    .Include(b => b.Reviews)
                    .Where(b => b.Title.Contains(query))
                    .ToListAsync();

                return books;
            }

            return BadRequest("Search query is required.");
        }

        private readonly ApplicationDbContext Context;
    }
}
