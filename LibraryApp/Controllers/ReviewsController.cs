using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace LibraryApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> AddReview([FromBody] ReviewRequest request)
        {
            var review = new Review
            {
                BookId = request.BookId,
                Message = request.Message,
                Rating = request.Rating,
                UserId = request.UserId,
                CreatedAt = System.DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Review added successfully." });
        }

        [HttpGet("{bookId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews(int bookId)
        {
            return await _context.Reviews
                .Where(r => r.BookId == bookId)
                .ToListAsync();
        }
    }

    public class ReviewRequest
    {
        public int BookId { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string UserId { get; set; }
    }
}
