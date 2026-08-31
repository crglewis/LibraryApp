using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibraryApp.Models;

#pragma warning disable CS8604

namespace LibraryApp.Controllers
{
   [ApiController]
   [Route("api/[controller]")]
   [Authorize]
   public class AuthController : ControllerBase
   {
      private readonly UserManager<ApplicationUser> _userManager;
      private readonly RoleManager<IdentityRole> _roleManager;
      private readonly SignInManager<ApplicationUser> _signInManager;

      public AuthController(UserManager<ApplicationUser> userManager,
                            RoleManager<IdentityRole> roleManager,
                            SignInManager<ApplicationUser> signInManager)
      {
         _userManager = userManager;
         _roleManager = roleManager;
         _signInManager = signInManager;
      }

      [HttpPost("register")]
      [AllowAnonymous]
      public async Task<IActionResult> Register([FromBody] RegisterRequest request)
      {
         if (request.Role != "Librarian" && request.Role != "Customer")
         {
            return BadRequest("Invalid role specified.");
         }

         var user = new ApplicationUser
         {
            UserName = request.Email,
            Email = request.Email,
            Role = request.Role
         };

         var result = await _userManager.CreateAsync(user, request.Password);

         if (!result.Succeeded)
         {
            return BadRequest(result.Errors);
         }

         if (!await _roleManager.RoleExistsAsync(request.Role))
         {
            await _roleManager.CreateAsync(new IdentityRole(request.Role));
         }

         var roleResult = await _userManager.AddToRolesAsync(user, new[] { request.Role });
         if (!roleResult.Succeeded)
         {
            return BadRequest(roleResult.Errors);
         }

         return Ok(new { Message = "User registered successfully." });
      }

      [HttpPost("login")]
      [AllowAnonymous]
      public async Task<IActionResult> Login([FromBody] LoginRequest request)
      {
         var user = await _userManager.FindByEmailAsync(request.Email);
         if (user == null)
         {
            return Unauthorized(new { Message = "Invalid credentials." });
         }

         // Skip login if username is empty or password is missing
         if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(request.Password))
         {
            return Unauthorized(new { Message = "Invalid credentials." });
         }

         var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, false);
         if (result.Succeeded)
         {
            return Ok(new { Message = "Login successful." });
         }

         return Unauthorized(new { Message = "Invalid credentials." });
      }

      [HttpPost("logout")]
      public async Task<IActionResult> Logout()
      {
         await _signInManager.SignOutAsync();
         return Ok(new { Message = "Logged out." });
      }

      [HttpGet("me")]
      public async Task<ActionResult> Me()
      {
         var user = await _userManager.GetUserAsync(User);
         if (user == null)
         {
            return Unauthorized();
         }

         return Ok(new { Id = user.Id, Email = user.Email, Role = user.Role });
      }
   }
}

#pragma warning restore CS8604
