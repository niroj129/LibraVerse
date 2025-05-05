namespace LibraVerse.DTOs.Discount;

public class CreateDiscountDto
{
    public Guid BookId { get; set; }
    
    public bool MarkAsSale { get; set; }
    
    public decimal DiscountPercentage { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
}