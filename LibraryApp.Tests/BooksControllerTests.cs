using LibraryApp.Controllers;
using LibraryApp.Models;
using LibraryApp.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Tests
{
    public class BooksControllerTests
    {
        private static Book MakeBook(string title, string author = "Author", bool isAvailable = true) => new()
        {
            Title = title,
            Author = author,
            CoverImage = "cover.jpg",
            ISBN = "0000000000",
            IsAvailable = isAvailable,
        };

        [Fact]
        public async Task GetBooks_ReturnsAllBooksInDatabase()
        {
            await using var context = TestDbContextFactory.Create();
            context.Books.AddRange(MakeBook("Book One"), MakeBook("Book Two"));
            await context.SaveChangesAsync();
            var controller = new BooksController(context);

            var result = await controller.GetBooks();

            var books = Assert.IsAssignableFrom<IEnumerable<Book>>(result.Value);
            Assert.Equal(2, books.Count());
        }

        [Fact]
        public async Task GetFeaturedBooks_ReturnsRequestedNumberOfDistinctBooks_WithReviews()
        {
            await using var context = TestDbContextFactory.Create();
            context.Books.AddRange(Enumerable.Range(1, 10).Select(i => MakeBook($"Book {i}")));
            await context.SaveChangesAsync();
            var first = await context.Books.FirstAsync();
            context.Reviews.Add(new Review { BookId = first.Id, UserId = "u1", Message = "Good", Rating = 4 });
            await context.SaveChangesAsync();
            var controller = new BooksController(context);

            var result = await controller.GetFeaturedBooks(count: 5);

            var books = Assert.IsAssignableFrom<IEnumerable<Book>>(result.Value).ToList();
            Assert.Equal(5, books.Count);
            Assert.Equal(5, books.Select(b => b.Id).Distinct().Count());
            var featuredFirst = books.SingleOrDefault(b => b.Id == first.Id);
            if (featuredFirst != null) Assert.Single(featuredFirst.Reviews);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(-3, 1)]
        [InlineData(500, 10)]
        public async Task GetFeaturedBooks_ClampsCount(int requested, int expected)
        {
            await using var context = TestDbContextFactory.Create();
            context.Books.AddRange(Enumerable.Range(1, 10).Select(i => MakeBook($"Book {i}")));
            await context.SaveChangesAsync();
            var controller = new BooksController(context);

            var result = await controller.GetFeaturedBooks(requested);

            Assert.Equal(expected, Assert.IsAssignableFrom<IEnumerable<Book>>(result.Value).Count());
        }

        [Fact]
        public async Task GetBook_ReturnsBook_WhenItExists()
        {
            await using var context = TestDbContextFactory.Create();
            var book = MakeBook("Findable Book");
            context.Books.Add(book);
            await context.SaveChangesAsync();
            var controller = new BooksController(context);

            var result = await controller.GetBook(book.Id);

            Assert.Equal("Findable Book", result.Value?.Title);
        }

        [Fact]
        public async Task GetBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            await using var context = TestDbContextFactory.Create();
            var controller = new BooksController(context);

            var result = await controller.GetBook(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateBook_PersistsBookToDatabase()
        {
            await using var context = TestDbContextFactory.Create();
            var controller = new BooksController(context);
            var newBook = MakeBook("New Arrival");

            var result = await controller.CreateBook(newBook);

            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(1, await context.Books.CountAsync());
        }

        [Fact]
        public async Task DeleteBook_RemovesBook_WhenItExists()
        {
            await using var context = TestDbContextFactory.Create();
            var book = MakeBook("To Be Deleted");
            context.Books.Add(book);
            await context.SaveChangesAsync();
            var controller = new BooksController(context);

            var result = await controller.DeleteBook(book.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(0, await context.Books.CountAsync());
        }

        [Fact]
        public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            await using var context = TestDbContextFactory.Create();
            var controller = new BooksController(context);

            var result = await controller.DeleteBook(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateBook_AppliesEditedFields()
        {
            await using var context = TestDbContextFactory.Create();
            var book = MakeBook("Original Title");
            context.Books.Add(book);
            await context.SaveChangesAsync();
            var controller = new BooksController(context);
            var edited = MakeBook("Updated Title");
            edited.Id = book.Id;

            var result = await controller.UpdateBook(book.Id, edited);

            Assert.Equal("Updated Title", result.Value?.Title);
        }

        [Fact]
        public async Task UpdateBook_DoesNotChangeAvailability_WhenBookIsCheckedOut()
        {
            await using var context = TestDbContextFactory.Create();
            var book = MakeBook("Checked Out Book", isAvailable: false);
            context.Books.Add(book);
            await context.SaveChangesAsync();
            var controller = new BooksController(context);

            // Mirrors the inventory edit form, which doesn't send isAvailable at all,
            // so it comes across as the Book model's default (true).
            var edited = MakeBook("Checked Out Book (edited)");
            edited.Id = book.Id;

            var result = await controller.UpdateBook(book.Id, edited);

            Assert.False(result.Value?.IsAvailable);
        }

        [Fact]
        public async Task SearchBooks_ReturnsOnlyBooksWithMatchingPartialTitle()
        {
            await using var context = TestDbContextFactory.Create();
            context.Books.AddRange(
                MakeBook("The Great Gatsby"),
                MakeBook("Great Expectations"),
                MakeBook("Moby Dick"));
            await context.SaveChangesAsync();
            var controller = new BooksController(context);

            var result = await controller.SearchBooks("Great");

            var books = Assert.IsAssignableFrom<IEnumerable<Book>>(result.Value);
            Assert.Equal(2, books.Count());
            Assert.DoesNotContain(books, b => b.Title == "Moby Dick");
        }

        [Fact]
        public async Task SearchBooks_IncludesReviews_SoClientsCanComputeAverageRating()
        {
            var databaseName = Guid.NewGuid().ToString();
            await using var context = TestDbContextFactory.Create(databaseName);
            var book = MakeBook("Reviewed Book");
            context.Books.Add(book);
            await context.SaveChangesAsync();
            context.Reviews.AddRange(
                new Review { BookId = book.Id, UserId = "u1", Message = "Great", Rating = 5 },
                new Review { BookId = book.Id, UserId = "u2", Message = "Fine", Rating = 3 });
            await context.SaveChangesAsync();

            // Fresh context so the result reflects what the query loads, not what's already tracked.
            await using var queryContext = TestDbContextFactory.Create(databaseName);
            var controller = new BooksController(queryContext);

            var result = await controller.SearchBooks("Reviewed");

            var found = Assert.Single(Assert.IsAssignableFrom<IEnumerable<Book>>(result.Value));
            Assert.Equal(2, found.Reviews.Count);
        }
    }
}
