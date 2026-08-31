using LibraryApp.Models;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace LibraryApp.Tests.TestHelpers
{
    public static class MockUserManagerFactory
    {
        // UserManager<T> has no interface, so controllers under test take the concrete class.
        // Moq can still proxy it (its members are virtual) as long as we supply the store its
        // constructor requires; CallBase lets simple members like GetUserId run their real logic.
        public static Mock<UserManager<ApplicationUser>> Create()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            mgr.CallBase = true;
            return mgr;
        }
    }
}
