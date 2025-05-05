using LibraVerse.Models;
using LibraVerse.DTOs.Authors;

namespace LibraVerse.Helper.Extension;

public static class AuthorExtensionMethod
{
    public static AuthorDto ToAuthorDto(this Author author)
    {
        return new AuthorDto()
        {
            Id = author.Id,
            Name = author.Name,
            Biography = author.Biography,
            Email = author.Email,
            IsActive = !author.IsDeleted
        };
    }
}