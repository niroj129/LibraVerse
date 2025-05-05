using System.ComponentModel.DataAnnotations.Schema;
using LibraVerse.Models.Base;

namespace LibraVerse.Models;

public class BookAuthors : BaseEntity
{
    public Guid BookId { get; set; }
    
    public Guid AuthorId { get; set; }
    
    [ForeignKey(nameof(BookId))]
    public virtual Book? Book { get; set; }
    
    [ForeignKey(nameof(AuthorId))]
    public virtual Author? Author { get; set; }
}