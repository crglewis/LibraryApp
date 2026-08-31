using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LibraryApp.Data;
using LibraryApp.Models;

namespace LibraryApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                UserId = _userManager.GetUserId(User)!,
                CreatedAt = System.DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Review added successfully." });
        }

        [HttpGet("{bookId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews(int bookId)
        {
            return await (from r in _context.Reviews
                           join u in _context.Users on r.UserId equals u.Id
                           where r.BookId == bookId
                           orderby r.CreatedAt descending
                           select new ReviewDto
                           {
                               Id = r.Id,
                               BookId = r.BookId,
                               UserId = r.UserId,
                               UserName = u.Email ?? "Anonymous",
                               Message = r.Message,
                               Rating = r.Rating,
                               CreatedAt = r.CreatedAt
                           }).ToListAsync();
        }
    }

    public class ReviewRequest
    {
        public int BookId { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Rating { get; set; }
    }

    public class ReviewDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Rating { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }
}
