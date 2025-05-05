using LibraVerse.Models;
using LibraVerse.DTOs.Publications;

namespace LibraVerse.Helper.Extension;

public static class PublicationExtensionMethod
{
    public static PublicationDto ToPublicationDto(this Publication format)
    {
        return new PublicationDto
        {
            Id = format.Id,
            Title = format.Title,
            Description = format.Description,
            IsActive = !format.IsDeleted
        };
    }
}