using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty; // Identity User ID

        public DateTime CheckoutDate { get; set; } = DateTime.UtcNow;

        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public bool IsReturned { get; set; } = false;
    }
}
