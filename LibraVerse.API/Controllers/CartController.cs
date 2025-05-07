using LibraVerse.Models;
using LibraVerse.DTOs.Cart;
using LibraVerse.Attribute;
using LibraVerse.Persistence;
using Microsoft.AspNetCore.Mvc;
using LibraVerse.DTOs.Response;
using LibraVerse.DTOs.Pagination;
using LibraVerse.Controllers.Base;
using LibraVerse.Helper.Extension;
using Microsoft.EntityFrameworkCore;
using LibraVerse.Services.Interface;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraVerse.Controllers;

[Authorize]
public class CartController(ApplicationDbContext applicationDbContext, IUserService userService) : BaseController
{
    [HttpGet]
    [SwaggerOperation(OperationId = "GetAllCart")]
    public PaginatedResponse<CartDto> GetAllCart([FromQuery] PaginationQuery query)
    {
        var userId = userService.UserId;
        
        var user = applicationDbContext.Users.Find(userId) 
                   ?? throw new Exception("User not found.");
        
        var carts = applicationDbContext.Carts
            .Where(x => x.UserId == user.Id)
            .Include(x => x.Book)
            .ToList();

        var cartDetails = carts
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PaginatedResponse<CartDto>
        {
            Items = cartDetails.Select(x => x.ToCartDto(applicationDbContext)).ToList(),
            TotalCount = carts.Count(),
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    [HttpGet("list")]
    [SwaggerOperation(OperationId = "GetAllCartList")]
    public List<CartDto> GetAllCart()
    {
        var userId = userService.UserId;
        
        var user = applicationDbContext.Users.Find(userId) 
                   ?? throw new Exception("User not found.");
        
        var carts = applicationDbContext.Carts
            .Where(x => x.UserId == user.Id)
            .Include(x => x.Book)
            .ToList();
        
        return carts.Select(x => x.ToCartDto(applicationDbContext)).ToList();
    }
    
    [HttpPost]
    [SwaggerOperation(OperationId = "AddToCart")]
    public Guid AddToCart(CreateCartDto cart)
    {
        var userId = userService.UserId;
        
        var user = applicationDbContext.Users.Find(userId) 
                   ?? throw new Exception("User not found.");
        
        var book = applicationDbContext.Books.Find(cart.BookId) 
                   ?? throw new Exception("Book not found.");

        Guid cartId;
        
        var existingCartModel = applicationDbContext.Carts
            .FirstOrDefault(x => x.UserId == user.Id && x.BookId == book.Id);

        if (book is { IsAvailable: false, Stock: 0 })
        {
            throw new Exception("Book is not available.");
        }
        
        if (existingCartModel == null)
        {
            var cartModel = new Cart()
            {
                UserId = user.Id,
                BookId = book.Id,
                Count = 1
            };
            
            applicationDbContext.Carts.Add(cartModel);
            applicationDbContext.SaveChanges();
            
            cartId = cartModel.Id;
        }
        else
        {
            existingCartModel.Count++;
            
            applicationDbContext.Carts.Update(existingCartModel);
            applicationDbContext.SaveChanges();
            
            cartId = existingCartModel.Id;
        }
        
        
        return cartId;
    }
    
    [HttpDelete("{cartId:guid}/book/{bookId:guid}")]
    [SwaggerOperation(OperationId = "RemoveFromCart")]
    public Guid RemoveFromCart(Guid cartId, Guid bookId)
    {
        var book = applicationDbContext.Books.Find(bookId) 
                   ?? throw new Exception("Book not found.");

        var cart = applicationDbContext.Carts.FirstOrDefault(x => x.Id == cartId && x.BookId == book.Id)
            ?? throw new Exception("Cart not found.");

        cart.Count--;
        
        applicationDbContext.Carts.Update(cart);
        applicationDbContext.SaveChanges();
        
        return cart.Id;
    }
    
    [HttpDelete("{cartId:guid}")]
    [SwaggerOperation(OperationId = "RemoveCart")]
    public Guid RemoveCart(Guid cartId)
    {
        var cart = applicationDbContext.Carts.Find(cartId) 
                   ?? throw new Exception("Cart not found.");

        applicationDbContext.Carts.Remove(cart);
        applicationDbContext.SaveChanges();
        
        return cart.Id;
    }
}