namespace LibraVerse.DTOs.Reviews;

public class CreateReviewDto
{
    public Guid BookId { get; set; }
    
    public int Rating { get; set; }
    
    public string Text { get; set; } = string.Empty;
}