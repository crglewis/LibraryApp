using LibraryApp.Tests.TestHelpers;

namespace LibraryApp.Tests
{
    public class DatabaseSeedingTests
    {
        [Fact]
        public void SeedData_PopulatesThirtyBooks_WhenDatabaseIsEmpty()
        {
            using var context = TestDbContextFactory.Create();

            context.SeedData();

            Assert.Equal(30, context.Books.Count());
        }

        [Fact]
        public void SeedData_GeneratesBooksWithRequiredFieldsPopulated()
        {
            using var context = TestDbContextFactory.Create();

            context.SeedData();

            Assert.All(context.Books, book =>
            {
                Assert.False(string.IsNullOrWhiteSpace(book.Title));
                Assert.False(string.IsNullOrWhiteSpace(book.Author));
                Assert.False(string.IsNullOrWhiteSpace(book.ISBN));
                Assert.False(string.IsNullOrWhiteSpace(book.CoverImage));
                Assert.InRange(book.PageCount, 100, 500);
            });
        }

        [Fact]
        public void SeedData_DoesNotDuplicate_WhenBooksAlreadyExist()
        {
            using var context = TestDbContextFactory.Create();
            context.SeedData();

            context.SeedData();

            Assert.Equal(30, context.Books.Count());
        }
    }
}
