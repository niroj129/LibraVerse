using LibraVerse.Models;
using LibraVerse.Attribute;
using LibraVerse.Persistence;
using Microsoft.AspNetCore.Mvc;
using LibraVerse.DTOs.Response;
using LibraVerse.DTOs.Wishlists;
using LibraVerse.DTOs.Pagination;
using LibraVerse.Controllers.Base;
using LibraVerse.Helper.Extension;
using LibraVerse.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraVerse.Controllers;

[Authorize]
public class WishlistController(ApplicationDbContext applicationDbContext, IUserService userService) : BaseController
{
    [HttpGet]
    [SwaggerOperation(OperationId = "GetAllWishlists")]
    public PaginatedResponse<WishlistDto> GetAllWishlists([FromQuery] PaginationQuery query)
    {
        var wishlists = applicationDbContext.Wishlists
            .Where(x => x.UserId == userService.UserId)
            .Include(x => x.Book)
            .AsQueryable();

        var wishlistDetails = wishlists
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PaginatedResponse<WishlistDto>
        {
            Items = wishlistDetails.Select(x => x.ToWishlistDto(applicationDbContext)).ToList(),
            TotalCount = wishlists.Count(),
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
    
    [HttpGet("list")]
    [SwaggerOperation(OperationId = "GetAllWishlistsList")]
    public List<WishlistDto> GetAllWishlists()
    {
        var wishlists = applicationDbContext.Wishlists
            .Where(x => x.UserId == userService.UserId)
            .Include(x => x.Book)
            .ToList()
            .AsQueryable();

        return wishlists.Select(x => x.ToWishlistDto(applicationDbContext)).ToList();
    }
    
    [HttpPost]
    [SwaggerOperation(OperationId = "CreateWishlist")]
    public Guid CreateWishlist(CreateWishlistDto wishlist)
    {
        var userId = userService.UserId;
        
        var user = applicationDbContext.Users.Find(userId)
            ?? throw new Exception("User not found.");
        
        var book = applicationDbContext.Books.Find(wishlist.BookId)
            ?? throw new Exception("Book not found.");
        
        var existingWishlist = applicationDbContext.Wishlists
            .FirstOrDefault(x => x.UserId == user.Id && x.BookId == book.Id);

        if (existingWishlist != null)
        {
            throw new Exception("Book already exists in the wishlist.");
        }
        
        var wishlistModel = new Wishlist
        {
            BookId = book.Id,
            UserId = user.Id
        };
        
        applicationDbContext.Wishlists.Add(wishlistModel);
        applicationDbContext.SaveChanges();

        return wishlistModel.Id;
    }
    
    [HttpDelete("{wishlistId:guid}")]
    [SwaggerOperation(OperationId = "DeleteWishlist")]
    public Guid DeleteWishlist(Guid wishlistId)
    {
        var wishlist = applicationDbContext.Wishlists.Find(wishlistId)
            ?? throw new Exception("Wishlist not found.");
        
        applicationDbContext.Wishlists.Remove(wishlist);
        applicationDbContext.SaveChanges();

        return wishlist.Id;
    }
}