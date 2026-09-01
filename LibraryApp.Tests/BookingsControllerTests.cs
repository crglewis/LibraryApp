using System.Security.Claims;
using LibraryApp.Controllers;
using LibraryApp.Hubs;
using LibraryApp.Models;
using LibraryApp.Tests.TestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LibraryApp.Tests
{
    public class BookingsControllerTests
    {
        private const string TestUserId = "test-user-id";

        private static BookingsController MakeController(LibraryApp.Data.ApplicationDbContext context)
        {
            var userManagerMock = MockUserManagerFactory.Create();

            // The controller only ever calls Clients.All.SendAsync(...) to broadcast; stub just
            // enough of the hub context chain that the call doesn't throw a null reference.
            var clientProxyMock = new Mock<IClientProxy>();
            clientProxyMock
                .Setup(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);
                
            var hubClientsMock = new Mock<IHubClients>();
            hubClientsMock.Setup(c => c.All).Returns(clientProxyMock.Object);
            
            var hubContextMock = new Mock<IHubContext<BookHub>>();
            hubContextMock.Setup(h => h.Clients).Returns(hubClientsMock.Object);

            var controller = new BookingsController(context, userManagerMock.Object, hubContextMock.Object);

            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, TestUserId) }, "TestAuth"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            return controller;
        }

        private static Book MakeAvailableBook(bool isAvailable = true) => new()
        {
            Title = "Checkoutable Book",
            Author = "Author",
            CoverImage = "cover.jpg",
            ISBN = "1111111111",
            IsAvailable = isAvailable,
        };

        [Fact]
        public async Task CheckoutBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            await using var context = TestDbContextFactory.Create();
            var controller = MakeController(context);

            var result = await controller.CheckoutBook(new BookingRequest { BookId = 999 });

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CheckoutBook_ReturnsBadRequest_WhenBookAlreadyCheckedOut()
        {
            await using var context = TestDbContextFactory.Create();
            var book = MakeAvailableBook(isAvailable: false);
            context.Books.Add(book);
            await context.SaveChangesAsync();
            var controller = MakeController(context);

            var result = await controller.CheckoutBook(new BookingRequest { BookId = book.Id });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CheckoutBook_MarksBookUnavailable_AndSetsDueDateFiveDaysOut()
        {
            await using var context = TestDbContextFactory.Create();
            var book = MakeAvailableBook();
            context.Books.Add(book);
            await context.SaveChangesAsync();
            var controller = MakeController(context);
            var beforeCheckout = DateTime.UtcNow;

            var result = await controller.CheckoutBook(new BookingRequest { BookId = book.Id });

            Assert.IsType<OkObjectResult>(result);
            var updatedBook = await context.Books.FindAsync(book.Id);
            Assert.False(updatedBook!.IsAvailable);

            var booking = await context.Bookings.SingleAsync(b => b.BookId == book.Id);
            Assert.Equal(TestUserId, booking.UserId);
            Assert.False(booking.IsReturned);
            Assert.InRange(booking.DueDate, beforeCheckout.AddDays(5), beforeCheckout.AddDays(5).AddSeconds(5));
        }

        [Fact]
        public async Task ReturnBook_ReturnsBadRequest_WhenNoActiveCheckoutExists()
        {
            await using var context = TestDbContextFactory.Create();
            var book = MakeAvailableBook();
            context.Books.Add(book);
            await context.SaveChangesAsync();
            var controller = MakeController(context);

            var result = await controller.ReturnBook(new ReturnRequest { BookId = book.Id });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ReturnBook_MarksBookAvailable_AndClosesOutBooking()
        {
            await using var context = TestDbContextFactory.Create();
            var book = MakeAvailableBook(isAvailable: false);
            context.Books.Add(book);
            context.Bookings.Add(new Booking
            {
                BookId = book.Id,
                Book = book,
                UserId = TestUserId,
                CheckoutDate = DateTime.UtcNow.AddDays(-1),
                DueDate = DateTime.UtcNow.AddDays(4),
                IsReturned = false,
            });
            await context.SaveChangesAsync();
            var controller = MakeController(context);

            var result = await controller.ReturnBook(new ReturnRequest { BookId = book.Id });

            Assert.IsType<OkObjectResult>(result);
            var updatedBook = await context.Books.FindAsync(book.Id);
            Assert.True(updatedBook!.IsAvailable);

            var booking = await context.Bookings.SingleAsync(b => b.BookId == book.Id);
            Assert.True(booking.IsReturned);
            Assert.NotNull(booking.ReturnDate);
        }

        [Fact]
        public async Task GetAllBookings_ReturnsBookingsNewestCheckoutFirst()
        {
            await using var context = TestDbContextFactory.Create();
            var bookA = MakeAvailableBook(isAvailable: false);
            var bookB = MakeAvailableBook(isAvailable: false);
            context.Books.AddRange(bookA, bookB);
            context.Bookings.AddRange(
                new Booking { Book = bookA, BookId = bookA.Id, UserId = TestUserId, CheckoutDate = DateTime.UtcNow.AddDays(-3), DueDate = DateTime.UtcNow.AddDays(2) },
                new Booking { Book = bookB, BookId = bookB.Id, UserId = TestUserId, CheckoutDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(5) });
            await context.SaveChangesAsync();
            var controller = MakeController(context);

            var result = await controller.GetAllBookings();

            var bookings = Assert.IsAssignableFrom<IEnumerable<Booking>>(result.Value).ToList();
            Assert.Equal(2, bookings.Count);
            Assert.Equal(bookB.Id, bookings[0].BookId);
        }
    }
}
