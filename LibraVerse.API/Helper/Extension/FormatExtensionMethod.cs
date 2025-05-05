using LibraVerse.Models;
using LibraVerse.DTOs.Formats;

namespace LibraVerse.Helper.Extension;

public static class FormatExtensionMethod
{
    public static FormatDto ToFormatDto(this Format format)
    {
        return new FormatDto
        {
            Id = format.Id,
            Title = format.Title,
            Description = format.Description,
            IsActive = !format.IsDeleted
        };
    }
}