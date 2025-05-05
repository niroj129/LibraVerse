using LibraVerse.Enums;
using LibraVerse.DTOs.User;
using LibraVerse.DTOs.Books;

namespace LibraVerse.DTOs.Order;

public class OrderDto
{
    public Guid Id { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    public OrderStatus Status { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public decimal DiscountPercentage { get; set; }
    
    public decimal GrandTotal { get; set; }

    public UserDto User { get; set; } = new();
    
    public List<OrderDetailsDto> OrderDetails { get; set; } = [];
}

public class OrderDetailsDto
{
    public Guid Id { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal NetTotal { get; set; }

    public decimal BookDiscount { get; set; }

    public decimal GrandTotal { get; set; }

    public BookDto Book { get; set; } = new();
}