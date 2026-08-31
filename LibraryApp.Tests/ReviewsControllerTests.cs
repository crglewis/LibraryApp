using System.Security.Claims;
using LibraryApp.Controllers;
using LibraryApp.Models;
using LibraryApp.Tests.TestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LibraryApp.Tests
{
    public class ReviewsControllerTests
    {
        private const string TestUserId = "test-user-id";

        private static ReviewsController MakeController(LibraryApp.Data.ApplicationDbContext context)
        {
            var userManagerMock = MockUserManagerFactory.Create();
            userManagerMock.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(TestUserId);

            var controller = new ReviewsController(context, userManagerMock.Object);

            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, TestUserId) }, "TestAuth"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal },
            };

            return controller;
        }

        private static Book MakeBook(string title = "Reviewable Book") => new()
        {
            Title = title,
            Author = "Author",
            CoverImage = "cover.jpg",
            ISBN = "2222222222",
            IsAvailable = true,
        };

        [Fact]
        public async Task AddReview_PersistsReview_ForCurrentUser()
        {
            await using var context = TestDbContextFactory.Create();
            var book = MakeBook();
            context.Books.Add(book);
            await context.SaveChangesAsync();
            var controller = MakeController(context);

            var result = await controller.AddReview(new ReviewRequest { BookId = book.Id, Message = "Loved it!", Rating = 5 });

            Assert.IsType<OkObjectResult>(result);
            var review = await context.Reviews.SingleAsync(r => r.BookId == book.Id);
            Assert.Equal(TestUserId, review.UserId);
            Assert.Equal("Loved it!", review.Message);
            Assert.Equal(5, review.Rating);
        }

        [Fact]
        public async Task GetReviews_ReturnsOnlyReviewsForRequestedBook_NewestFirst()
        {
            await using var context = TestDbContextFactory.Create();
            var bookA = MakeBook("Book A");
            var bookB = MakeBook("Book B");
            context.Books.AddRange(bookA, bookB);
            var user = new ApplicationUser { Id = TestUserId, UserName = "reviewer@b.com", Email = "reviewer@b.com", Role = "Customer" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            context.Reviews.AddRange(
                new Review { BookId = bookA.Id, UserId = TestUserId, Message = "Older review", Rating = 3, CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new Review { BookId = bookA.Id, UserId = TestUserId, Message = "Newer review", Rating = 5, CreatedAt = DateTime.UtcNow },
                new Review { BookId = bookB.Id, UserId = TestUserId, Message = "Wrong book", Rating = 1, CreatedAt = DateTime.UtcNow });
            await context.SaveChangesAsync();
            var controller = MakeController(context);

            var result = await controller.GetReviews(bookA.Id);

            var reviews = Assert.IsAssignableFrom<IEnumerable<ReviewDto>>(result.Value).ToList();
            Assert.Equal(2, reviews.Count);
            Assert.Equal("Newer review", reviews[0].Message);
            Assert.Equal("Older review", reviews[1].Message);
            Assert.All(reviews, r => Assert.Equal("reviewer@b.com", r.UserName));
        }

        [Fact]
        public async Task GetReviews_ReturnsEmpty_WhenBookHasNoReviews()
        {
            await using var context = TestDbContextFactory.Create();
            var book = MakeBook();
            context.Books.Add(book);
            await context.SaveChangesAsync();
            var controller = MakeController(context);

            var result = await controller.GetReviews(book.Id);

            var reviews = Assert.IsAssignableFrom<IEnumerable<ReviewDto>>(result.Value);
            Assert.Empty(reviews);
        }
    }
}
