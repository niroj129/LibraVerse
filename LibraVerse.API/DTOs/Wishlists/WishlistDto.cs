using LibraVerse.DTOs.Books;

namespace LibraVerse.DTOs.Wishlists;

public class WishlistDto
{
    public Guid Id { get; set; }
    
    public BookDto Book { get; set; } = new();
    
    public DateTime RegisteredDate { get; set; }
}