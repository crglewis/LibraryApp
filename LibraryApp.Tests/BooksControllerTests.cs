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
    }
}
