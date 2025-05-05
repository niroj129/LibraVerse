using LibraVerse.Models.Base;

namespace LibraVerse.Models;

public class Author : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public string Biography { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
}