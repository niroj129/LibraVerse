using LibraVerse.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;
using LibraVerse.Enums;

namespace LibraVerse.Models;

public class Announcement : BaseEntity
{
    public Guid? BookId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public AnnouncementType Type { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    [ForeignKey(nameof(BookId))]
    public virtual Book? Book { get; set; }
}