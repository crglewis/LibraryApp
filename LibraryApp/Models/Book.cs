using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApp.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string CoverImage { get; set; } = string.Empty;

        public string Publisher { get; set; } = string.Empty;

        public DateTime PublicationDate { get; set; }

        public string Category { get; set; } = string.Empty;

        [Required]
        public string ISBN { get; set; } = string.Empty;

        public int PageCount { get; set; }

        public bool IsAvailable { get; set; } = true;

        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
