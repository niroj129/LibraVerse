namespace LibraVerse.DTOs.Discount;

public class DiscountDto
{
    public Guid Id { get; set; }
    
    public bool MarkAsSale { get; set; }
    
    public decimal DiscountPercentage { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
}