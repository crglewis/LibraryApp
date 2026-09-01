using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LibraryApp.Data;
using LibraryApp.Models;

namespace LibraryApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            Context = context;
            UserManager = userManager;
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
                UserId = UserManager.GetUserId(User)!,
                CreatedAt = System.DateTime.UtcNow
            };

            Context.Reviews.Add(review);
            await Context.SaveChangesAsync();

            return Ok(new { Message = "Review added successfully." });
        }

        [HttpGet("{bookId}")]
        public async Task<ActionResult<List<ReviewDto>>> GetReviews(int bookId)
        {
            return await (from r in Context.Reviews
                          join u in Context.Users on r.UserId equals u.Id
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

        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager;
    }
}
