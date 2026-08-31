using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using LibraryApp.Data;
using LibraryApp.Hubs;
using LibraryApp.Models;

namespace LibraryApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<BookHub> _bookHub;

        public BookingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHubContext<BookHub> bookHub)
        {
            _context = context;
            _userManager = userManager;
            _bookHub = bookHub;
        }

        [HttpGet]
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings()
        {
            return await _context.Bookings
                .Include(b => b.Book)
                .OrderByDescending(b => b.CheckoutDate)
                .ToListAsync();
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
                UserId = _userManager.GetUserId(User)!,
                CheckoutDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(5),
                IsReturned = false
            };

            _context.Bookings.Add(booking);
            book.IsAvailable = false;

            await _context.SaveChangesAsync();

            await _bookHub.Clients.All.SendAsync("BookAvailabilityChanged", new { bookId = book.Id, isAvailable = false });

            return Ok(new { Message = "Book checked out successfully.", DueDate = booking.DueDate });
        }

        [HttpPost("returns")]
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult> ReturnBook([FromBody] ReturnRequest request)
        {
            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookId == request.BookId && b.IsReturned == false);

            if (booking == null)
            {
                return BadRequest("No active checkout found for this book.");
            }

            book.IsAvailable = true;
            booking.IsReturned = true;
            booking.ReturnDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _bookHub.Clients.All.SendAsync("BookAvailabilityChanged", new { bookId = book.Id, isAvailable = true });

            return Ok(new { Message = "Book returned successfully." });
        }
    }
}
