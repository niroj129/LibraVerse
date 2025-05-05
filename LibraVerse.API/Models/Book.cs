using LibraVerse.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;
using LibraVerse.Enums;

namespace LibraVerse.Models;

public class Book : BaseEntity
{
    public Guid PublicationId { get; set; }
    
    public Guid FormatId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string Iban { get; set; } = string.Empty;
    
    public Language Language { get; set; }
    
    public Genre Genre { get; set; }
    
    public decimal Price { get; set; }
    
    public bool IsAvailable { get; set; }
    
    public int Stock { get; set; }
    
    public DateTime PublishedDate { get; set; }
    
    public string CoverImage { get; set; } = string.Empty;
    
    [ForeignKey(nameof(PublicationId))]
    public virtual Publication? Publication { get; set; }
    
    [ForeignKey(nameof(FormatId))]
    public virtual Format? Format { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = [];

    public virtual ICollection<Discount> Discounts { get; set; } = [];
    
    public virtual ICollection<BookAuthors> BookAuthors { get; set; } = [];
}