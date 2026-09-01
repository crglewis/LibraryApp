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
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private const int CheckoutPeriodDays = 5;

        public BookingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IHubContext<BookHub> bookHub)
        {
            Context = context;
            UserManager = userManager;
            BookHub = bookHub;
        }

        [HttpGet]
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<List<Booking>>> GetAllBookings()
        {
            return await Context.Bookings
                .Include(b => b.Book)
                .OrderByDescending(b => b.CheckoutDate)
                .ToListAsync();
        }


        [HttpPost("returns")]
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult> ReturnBook([FromBody] ReturnRequest request)
        {
            var book = await Context.Books.FindAsync(request.BookId);
            var booking = await Context.Bookings
                .FirstOrDefaultAsync(b => b.BookId == request.BookId && b.IsReturned == false);

            if (book == null)
            {
                return NotFound("Book not found.");
            }

            if (booking == null)
            {
                return BadRequest("No active checkout found for this book.");
            }

            book.IsAvailable = true;
            booking.IsReturned = true;
            booking.ReturnDate = DateTime.UtcNow;

            await Context.SaveChangesAsync();

            await NotifyAvailabilityChanged(book.Id, isAvailable: true);

            return Ok(new { Message = "Book returned successfully." });
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> CheckoutBook([FromBody] BookingRequest request)
        {
            var book = await Context.Books.FindAsync(request.BookId);
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
                UserId = UserManager.GetUserId(User)!,
                CheckoutDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(CheckoutPeriodDays),
                IsReturned = false
            };

            Context.Bookings.Add(booking);
            book.IsAvailable = false;

            await Context.SaveChangesAsync();

            await NotifyAvailabilityChanged(book.Id, isAvailable: false);

            return Ok(new { Message = "Book checked out successfully.", DueDate = booking.DueDate });
        }

        private Task NotifyAvailabilityChanged(int bookId, bool isAvailable)
        {
            return BookHub.Clients.All.SendAsync("BookAvailabilityChanged", new { bookId, isAvailable });
        }

        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly IHubContext<BookHub> BookHub;
    }
}
