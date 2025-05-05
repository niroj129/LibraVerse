using LibraVerse.Enums;

namespace LibraVerse.DTOs.Authentication;

public class CreateUserDto : RegisterRequestDto
{
    public Role Role { get; set; }
}