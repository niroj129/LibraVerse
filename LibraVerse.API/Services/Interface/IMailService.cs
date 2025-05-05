using LibraVerse.DTOs.Email;

namespace LibraVerse.Services.Interface;

public interface IMailService
{
    void SendEmail(EmailDto email);
}