using Microsoft.AspNetCore.Identity;
using Moq;

namespace LibraryApp.Tests.TestHelpers
{
    public static class MockRoleManagerFactory
    {
        // Same rationale as MockUserManagerFactory: RoleManager<T> has no interface, but its
        // members are virtual so Moq can proxy it once the store its constructor requires is supplied.
        public static Mock<RoleManager<IdentityRole>> Create()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            var mgr = new Mock<RoleManager<IdentityRole>>(store.Object, null!, null!, null!, null!);
            mgr.CallBase = true;
            return mgr;
        }
    }
}
