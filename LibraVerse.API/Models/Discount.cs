using System.ComponentModel.DataAnnotations.Schema;
using LibraVerse.Models.Base;

namespace LibraVerse.Models;

public class Discount : BaseEntity
{
    public Guid BookId { get; set; }
    
    public bool MarkAsSale { get; set; }
    
    public decimal DiscountPercentage { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    [ForeignKey(nameof(BookId))]
    public virtual Book? Book { get; set; }
}