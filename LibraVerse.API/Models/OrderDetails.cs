using LibraVerse.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraVerse.Models;

public class OrderDetails : BaseEntity
{
    public Guid OrderId { get; set; }

    public Guid BookId { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal NetTotal { get; set; }

    public decimal BookDiscount { get; set; }

    public decimal GrandTotal { get; set; }
    
    [ForeignKey(nameof(OrderId))]
    public virtual Order? Order { get; set; }
    
    [ForeignKey(nameof(BookId))]
    public virtual Book? Book { get; set; }
}