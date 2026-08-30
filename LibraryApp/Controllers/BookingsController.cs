using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace LibraryApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> CheckoutBook([FromBody] BookingRequest request)
        {
            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }

            if (!book.IsAvailable)
            {
                return BadRequest("Book is already checked out.");
            }

            var booking = new Booking
            {
                BookId = request.BookId,
                UserId = request.UserId,
                CheckoutDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(5),
                IsReturned = false
            };

            _context.Bookings.Add(booking);
            book.IsAvailable = false;
            
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Book checked out successfully.", DueDate = booking.DueDate });
        }

        [HttpPost("returns")]
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult> ReturnBook(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookId == bookId && b.IsReturned == false);

            if (booking == null)
            {
                return BadRequest("No active checkout found for this book.");
            }

            book.IsAvailable = true;
            booking.IsReturned = true;
            booking.ReturnDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Book returned successfully." });
        }
    }
}
