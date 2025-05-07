using LibraVerse.Helper;
using LibraVerse.Models;
using LibraVerse.DTOs.User;
using LibraVerse.Attribute;
using LibraVerse.Persistence;
using LibraVerse.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using LibraVerse.DTOs.Pagination;
using LibraVerse.Controllers.Base;
using LibraVerse.Helper.Extension;
using LibraVerse.DTOs.Authentication;
using LibraVerse.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraVerse.Controllers;

[Authorize]
public class UserController(ApplicationDbContext applicationDbContext) : BaseController
{
    [HttpGet]
    [SwaggerOperation(OperationId = "GetAllUsers")]
    public PaginatedResponse<UserDto> GetAllUsers([FromQuery] PaginationQuery query, string? search = null, bool? isActive = null, Role? role = null)
    {
        var users = applicationDbContext.Users
            .AsQueryable();

        var userDetails = users
            .Where(x => string.IsNullOrEmpty(search) || x.Name.Contains(search) || x.Email.Contains(search))
            .Where(x => isActive == null || x.IsActive == isActive)
            .Where(x => role == null || x.Role == role)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();
        
        return new PaginatedResponse<UserDto>
        {
            Items = userDetails.Select(x => x.ToUserDto()).ToList(),
            TotalCount = users.Count(),
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
    
    [HttpGet("list")]
    [SwaggerOperation(OperationId = "GetAllUsersList")]
    public List<UserDto> GetAllUsers(string? search = null, bool? isActive = null, Role? role = null)
    {
        var users = applicationDbContext.Users
            .Where(x => string.IsNullOrEmpty(search) || x.Name.Contains(search) || x.Email.Contains(search))
            .Where(x => isActive == null || x.IsActive == isActive)
            .Where(x => role == null || x.Role == role)
            .AsQueryable();

        return users.Select(x => x.ToUserDto()).ToList();
    }
    
    [HttpGet("{userId:guid}")]
    [SwaggerOperation(OperationId = "GetUser")]
    public UserDto GetUser(Guid userId)
    {
        var user = applicationDbContext.Users.Find(userId)
            ?? throw new Exception("User not found.");
        
        return user.ToUserDto();
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "RegisterUser")]
    public Guid RegisterUser(CreateUserDto user)
    {
        if (applicationDbContext.Users.Any(u => u.Email == user.Email))
        {
            throw new Exception("An account with the respective email address has already been registered.");
        }

        var userModel = new User
        {
            Name = user.Name,
            Email = user.Email,
            Password = user.Password.Hash(),
            Role = user.Role,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            City = user.City,
            State = user.State,
            IsActive = true
        };

        applicationDbContext.Users.Add(userModel);
        applicationDbContext.SaveChanges();

        return userModel.Id;
    }
    
    [HttpPatch("{userId:guid}/status")]
    [SwaggerOperation(OperationId = "UpdateUserActivationStatus")]
    public Guid UpdateUserActivationStatus(Guid userId)
    {
        var user = applicationDbContext.Users.Find(userId)
            ?? throw new Exception("User not found.");
        
        user.IsActive = !user.IsActive;
        
        applicationDbContext.Users.Update(user);
        applicationDbContext.SaveChanges();
        
        return user.Id;
    }
}