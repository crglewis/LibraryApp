using Microsoft.AspNetCore.Identity;

namespace LibraryApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; } = string.Empty;
    }
}
