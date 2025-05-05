using LibraVerse.DTOs.User;

namespace LibraVerse.DTOs.Reviews;

public class ReviewDto
{
    public Guid Id { get; set; }
    
    public int Rating { get; set; }

    public string Text { get; set; } = string.Empty;

    public UserDto User { get; set; } = new();
}