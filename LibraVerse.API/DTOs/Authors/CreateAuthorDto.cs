namespace LibraVerse.DTOs.Authors;

public class CreateAuthorDto
{
    public string Name { get; set; } = string.Empty;
    
    public string Biography { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
}