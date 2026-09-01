namespace LibraryApp.Models
{
    public class ReviewRequest
    {
        public int BookId { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}
