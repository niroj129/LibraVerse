namespace LibraVerse.DTOs.Response;

public class ExceptionDto
{
    public int Status { get; set; }
    
    public string Error { get; set; } = string.Empty;
    
    public string Message { get; set; } = string.Empty;
    
    public DateTime Timestamp { get; set; } = DateTime.Now;
}