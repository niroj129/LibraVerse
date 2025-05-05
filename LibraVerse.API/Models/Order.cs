using System.ComponentModel.DataAnnotations.Schema;
using LibraVerse.Enums;
using LibraVerse.Models.Base;

namespace LibraVerse.Models;

public class Order : BaseEntity
{
    public Guid UserId { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    public OrderStatus Status { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public decimal DiscountPercentage { get; set; }
    
    public decimal GrandTotal { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }

    public virtual ICollection<OrderDetails> OrderDetails { get; set; } = [];
}