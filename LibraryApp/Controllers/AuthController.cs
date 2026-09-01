using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibraryApp.Models;

namespace LibraryApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            SignInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request.Role is not ("Librarian" or "Customer"))
            {
                return BadRequest("Invalid role specified.");
            }

            if (!await RoleManager.RoleExistsAsync(request.Role))
            {
                await RoleManager.CreateAsync(new IdentityRole(request.Role));
            }

            var applicationUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                Role = request.Role
            };

            var identityResult = await UserManager.CreateAsync(applicationUser, request.Password);
            if (!identityResult.Succeeded)
            {
                return BadRequest(identityResult.Errors);
            }

            var roleResult = await UserManager.AddToRolesAsync(applicationUser, [request.Role]);
            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }

            return Ok(new { Message = "User registered successfully." });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await UserManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid credentials." });
            }

            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(request.Password))
            {
                return Unauthorized(new { Message = "Invalid credentials." });
            }

            var result = await SignInManager.PasswordSignInAsync(user.UserName, request.Password, false, false);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Login successful." });
            }

            return Unauthorized(new { Message = "Invalid credentials." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return Ok(new { Message = "Logged out." });
        }

        [HttpGet("me")]
        public async Task<ActionResult> Me()
        {
            var user = await UserManager.GetUserAsync(User);
            if (user != null)
            {
                return Ok(new { user.Id, user.Email, user.Role });
            }

            return Unauthorized();
        }

        private readonly UserManager<ApplicationUser> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;
        private readonly SignInManager<ApplicationUser> SignInManager;
    }
}
