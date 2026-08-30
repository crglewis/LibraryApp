using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace LibraryApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<Book>> CreateBook([FromBody] Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<Book>> UpdateBook(int id, [FromBody] Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return book;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBooks([FromQuery] string query)
        {
            var books = await _context.Books
                .Where(b => b.Title.Contains(query))
                .ToListAsync();

            return books;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Book>>> FilterBooks(string author, string availability)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.Author.Contains(author));
            }

            if (!string.IsNullOrEmpty(availability))
            {
                if (availability.ToLower() == "true")
                {
                    query = query.Where(b => b.IsAvailable);
                }
                else if (availability.ToLower() == "false")
                {
                    query = query.Where(b => !b.IsAvailable);
                }
            }

            return await query.ToListAsync();
        }
    }
}
