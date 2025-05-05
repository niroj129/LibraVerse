using LibraVerse.Models;
using LibraVerse.Persistence;
using LibraVerse.DTOs.Announcement;

namespace LibraVerse.Helper.Extension;

public static class AnnouncementExtensionMethod
{
    public static AnnouncementDto ToAnnouncementDto(this Announcement announcement, ApplicationDbContext applicationDbContext)
    {
        return new AnnouncementDto()
        {
            Id = announcement.Id,
            Title = announcement.Title,
            Description = announcement.Description,
            Type = announcement.Type.ToString(),
            StartDate = announcement.StartDate,
            EndDate = announcement.EndDate,
            Book = announcement.Book?.ToBookDto(applicationDbContext)
        };
    }
}