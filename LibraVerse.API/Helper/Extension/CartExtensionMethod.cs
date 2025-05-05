using LibraVerse.Models;
using LibraVerse.DTOs.Books;
using LibraVerse.DTOs.Cart;
using LibraVerse.Persistence;

namespace LibraVerse.Helper.Extension;

public static class CartExtensionMethod
{
    public static CartDto ToCartDto(this Cart cart, ApplicationDbContext applicationDbContext)
    {
        return new CartDto
        {
            Id = cart.Id,
            Book = cart.Book?.ToBookDto(applicationDbContext) ?? new BookDto(),
            Quantity = cart.Count,
            TotalPrice = cart.Count * cart.Book?.Price ?? 0,
        };
    }
}