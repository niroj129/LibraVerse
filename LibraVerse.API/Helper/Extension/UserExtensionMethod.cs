using LibraVerse.Models;
using LibraVerse.DTOs.User;

namespace LibraVerse.Helper.Extension;

public static class UserExtensionMethod
{
    public static UserDto ToUserDto(this User user)
    {
        return new UserDto()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            City = user.City,
            State = user.State,
            IsActive = user.IsActive,
            Role = user.Role.ToString()
        };
    }
}