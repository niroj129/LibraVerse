using LibraVerse.DTOs.Books;

namespace LibraVerse.DTOs.Cart;

public class CartDto
{
    public Guid Id { get; set; }
    
    public BookDto Book { get; set; } = new();
    
    public int Quantity { get; set; } = 1;
    
    public decimal TotalPrice { get; set; } = 0;
}