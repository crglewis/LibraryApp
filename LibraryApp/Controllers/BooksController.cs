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
                Context.Entry(book).State = EntityState.Modified;

                await Context.SaveChangesAsync();

                return book;
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
                var books = await Context.Books
                    .Where(b => b.Title.Contains(query))
                    .ToListAsync();

                return books;
            }

            return BadRequest("Search query is required.");
        }

        private readonly ApplicationDbContext Context;
    }
}
