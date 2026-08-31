using LibraryApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace LibraryApp.Tests.TestHelpers
{
    public static class MockSignInManagerFactory
    {
        // SignInManager<T>'s constructor throws on a null IHttpContextAccessor/claims factory,
        // so those two get real mocks; the rest are unused by the methods AuthController calls
        // once PasswordSignInAsync/SignOutAsync are stubbed directly on the mock.
        public static Mock<SignInManager<ApplicationUser>> Create(UserManager<ApplicationUser> userManager)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(c => c.HttpContext).Returns(new DefaultHttpContext());
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

            var mgr = new Mock<SignInManager<ApplicationUser>>(
                userManager, contextAccessor.Object, claimsFactory.Object, null!, null!, null!, null!);
            mgr.CallBase = true;
            return mgr;
        }
    }
}
