using LibraVerse.Enums;

namespace LibraVerse.DTOs.Announcement;

public class UpdateAnnouncementDto
{
    public Guid? BookId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public AnnouncementType Type { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
}