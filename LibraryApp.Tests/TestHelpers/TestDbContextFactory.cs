using LibraryApp.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Tests.TestHelpers
{
    public static class TestDbContextFactory
    {
        // Each call gets its own isolated named database so tests never see each other's data.
        // Pass the same `databaseName` twice to get a second context (with an empty change
        // tracker) over the same data - needed when a test must prove a query actually loads
        // related entities rather than getting them for free from the seeding context's tracker.
        public static ApplicationDbContext Create(string? databaseName = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
