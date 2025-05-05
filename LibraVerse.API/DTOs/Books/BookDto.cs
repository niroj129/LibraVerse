using LibraVerse.DTOs.Authors;
using LibraVerse.DTOs.Discount;
using LibraVerse.DTOs.Formats;
using LibraVerse.DTOs.Publications;
using LibraVerse.DTOs.Reviews;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraVerse.DTOs.Books;

public class BookDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string Iban { get; set; } = string.Empty;
    
    public string Language { get; set; } = string.Empty;
    
    public string Genre { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public bool IsAvailable { get; set; }
    
    public int Stock { get; set; }
    
    public DateTime PublishedDate { get; set; }
    
    public string CoverImage { get; set; } = string.Empty;
    
    public bool IsAddedToWishlist { get; set; }
    
    public bool IsAddedToCart { get; set; }

    public bool IsPurchased { get; set; }
    
    public FormatDto Format { get; set; } = new();
    
    public PublicationDto Publication { get; set; } = new();
    
    [SwaggerSchema(Nullable = true)]
    public DiscountDto? Discount { get; set; }

    public List<AuthorDto> Authors { get; set; } = new();

    public List<ReviewDto> Reviews { get; set; } = new();

    public List<DiscountDto> Discounts { get; set; } = new();
}