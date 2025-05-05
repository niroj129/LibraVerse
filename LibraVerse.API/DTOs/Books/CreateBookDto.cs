using LibraVerse.Enums;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraVerse.DTOs.Books;

public class CreateBookDto
{
    public Guid FormatId { get; set; }
    
    public Guid PublicationId { get; set; }
    
    public List<Guid> AuthorIds { get; set; } = new();
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string Iban { get; set; } = string.Empty;
    
    public Language Language { get; set; }
    
    public Genre Genre { get; set; }
    
    public decimal Price { get; set; }
    
    public bool IsAvailable { get; set; }
    
    public int Stock { get; set; }
    
    public DateTime PublishedDate { get; set; }
    
    [SwaggerSchema(Nullable = true)]
    public IFormFile? CoverImage { get; set; }
}