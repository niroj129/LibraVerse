using System.ComponentModel.DataAnnotations.Schema;
using LibraVerse.Models.Base;

namespace LibraVerse.Models;

public class Wishlist : BaseEntity
{
    public Guid BookId { get; set; }
    
    public Guid UserId { get; set; }
    
    [ForeignKey(nameof(BookId))]
    public virtual Book? Book { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }
}