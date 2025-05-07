using LibraVerse.Enums;
using LibraVerse.Helper;
using LibraVerse.Models;
using LibraVerse.Persistence;
using Microsoft.AspNetCore.Mvc;
using LibraVerse.Controllers.Base;
using LibraVerse.Helper.Extension;
using LibraVerse.DTOs.Authentication;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraVerse.Controllers;

public class AuthenticationController(ApplicationDbContext applicationDbContext) : BaseController
{
    [HttpPost]
    [SwaggerOperation(OperationId = "Login")]
    public LoginResponseDto Login(LoginRequestDto loginRequest)
    {
        var user = applicationDbContext.Users.FirstOrDefault(u => u.Email == loginRequest.Email)
            ?? throw new Exception("An account with the respective email address has not been registered.");

        if (!loginRequest.Password.Verify(user.Password))
        {
            throw new Exception("Your password is incorrect, please try again.");
        }

        if (!user.IsActive)
        {
            throw new  Exception("Account is inactive, please contact your administrator.");
        }

        var token = JwtTokenGenerator.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            User = user.ToUserDto()
        };
    }
    
    [HttpPost("register")]
    [SwaggerOperation(OperationId = "Register")]
    public Guid Register(RegisterRequestDto registerRequest)
    {
        if (applicationDbContext.Users.Any(u => u.Email == registerRequest.Email))
        {
            throw new Exception("An account with the respective email address has already been registered.");
        }

        var user = new User
        {
            Name = registerRequest.Name,
            Email = registerRequest.Email,
            Password = registerRequest.Password.Hash(),
            Role = Role.Customer,
            PhoneNumber = registerRequest.PhoneNumber,
            Address = registerRequest.Address,
            City = registerRequest.City,
            State = registerRequest.State,
            IsActive = true
        };

        applicationDbContext.Users.Add(user);
        applicationDbContext.SaveChanges();

        return user.Id;
    }
}