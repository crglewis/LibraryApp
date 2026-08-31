using System.Security.Claims;
using LibraryApp.Controllers;
using LibraryApp.Models;
using LibraryApp.Tests.TestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LibraryApp.Tests
{
    public class AuthControllerTests
    {
        private static AuthController MakeController(
            out Mock<UserManager<ApplicationUser>> userManagerMock,
            out Mock<RoleManager<IdentityRole>> roleManagerMock,
            out Mock<SignInManager<ApplicationUser>> signInManagerMock)
        {
            userManagerMock = MockUserManagerFactory.Create();
            roleManagerMock = MockRoleManagerFactory.Create();
            signInManagerMock = MockSignInManagerFactory.Create(userManagerMock.Object);

            return new AuthController(userManagerMock.Object, roleManagerMock.Object, signInManagerMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRoleIsInvalid()
        {
            var controller = MakeController(out _, out _, out _);

            var result = await controller.Register(new RegisterRequest { Email = "a@b.com", Password = "Pass123!", Role = "Wizard" });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUserCreationFails()
        {
            var controller = MakeController(out var userManagerMock, out _, out _);
            userManagerMock
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak." }));

            var result = await controller.Register(new RegisterRequest { Email = "a@b.com", Password = "weak", Role = "Customer" });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_CreatesRoleAndAssignsIt_WhenRoleDoesNotExistYet()
        {
            var controller = MakeController(out var userManagerMock, out var roleManagerMock, out _);
            userManagerMock
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            roleManagerMock.Setup(m => m.RoleExistsAsync("Librarian")).ReturnsAsync(false);
            roleManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);
            userManagerMock
                .Setup(m => m.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.Is<IEnumerable<string>>(r => r.Single() == "Librarian")))
                .ReturnsAsync(IdentityResult.Success);

            var result = await controller.Register(new RegisterRequest { Email = "lib@b.com", Password = "Pass123!", Role = "Librarian" });

            Assert.IsType<OkObjectResult>(result);
            roleManagerMock.Verify(m => m.CreateAsync(It.Is<IdentityRole>(r => r.Name == "Librarian")), Times.Once);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRoleAssignmentFails()
        {
            var controller = MakeController(out var userManagerMock, out var roleManagerMock, out _);
            userManagerMock
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            roleManagerMock.Setup(m => m.RoleExistsAsync("Customer")).ReturnsAsync(true);
            userManagerMock
                .Setup(m => m.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Role assignment failed." }));

            var result = await controller.Register(new RegisterRequest { Email = "a@b.com", Password = "Pass123!", Role = "Customer" });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenUserDoesNotExist()
        {
            var controller = MakeController(out var userManagerMock, out _, out _);
            userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

            var result = await controller.Login(new LoginRequest { Email = "nobody@b.com", Password = "Pass123!" });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            var controller = MakeController(out var userManagerMock, out _, out var signInManagerMock);
            var user = new ApplicationUser { UserName = "a@b.com", Email = "a@b.com", Role = "Customer" };
            userManagerMock.Setup(m => m.FindByEmailAsync("a@b.com")).ReturnsAsync(user);
            signInManagerMock
                .Setup(m => m.PasswordSignInAsync("a@b.com", "Pass123!", false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var result = await controller.Login(new LoginRequest { Email = "a@b.com", Password = "Pass123!" });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenPasswordIsWrong()
        {
            var controller = MakeController(out var userManagerMock, out _, out var signInManagerMock);
            var user = new ApplicationUser { UserName = "a@b.com", Email = "a@b.com", Role = "Customer" };
            userManagerMock.Setup(m => m.FindByEmailAsync("a@b.com")).ReturnsAsync(user);
            signInManagerMock
                .Setup(m => m.PasswordSignInAsync("a@b.com", "WrongPass", false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var result = await controller.Login(new LoginRequest { Email = "a@b.com", Password = "WrongPass" });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Me_ReturnsUnauthorized_WhenNoUserIsAuthenticated()
        {
            var controller = MakeController(out var userManagerMock, out _, out _);
            userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((ApplicationUser?)null);

            var result = await controller.Me();

            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
