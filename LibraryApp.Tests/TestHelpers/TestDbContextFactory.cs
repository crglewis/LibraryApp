using LibraryApp.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Tests.TestHelpers
{
    public static class TestDbContextFactory
    {
        // Each call gets its own isolated named database so tests never see each other's data.
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
