using LibraVerse.Models.Base;

namespace LibraVerse.Models;

public class Format : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
}