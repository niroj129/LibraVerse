using System.ComponentModel.DataAnnotations.Schema;
using LibraVerse.Models.Base;

namespace LibraVerse.Models;

public class Review : BaseEntity
{
    public Guid BookId { get; set; }
    
    public Guid UserId { get; set; }
    
    public int Rating { get; set; }
    
    public string Text { get; set; } = string.Empty;
    
    [ForeignKey(nameof(BookId))]
    public virtual Book? Book { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }
}