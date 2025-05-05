using LibraVerse.Models;
using LibraVerse.DTOs.Books;
using LibraVerse.DTOs.Wishlists;
using LibraVerse.Persistence;

namespace LibraVerse.Helper.Extension;

public static class WishlistExtensionMethod
{
    public static WishlistDto ToWishlistDto(this Wishlist wishlist, ApplicationDbContext applicationDbContext)
    {
        return new WishlistDto()
        {
            Id = wishlist.Id,
            Book = wishlist.Book?.ToBookDto(applicationDbContext) ?? new BookDto(),
            RegisteredDate = wishlist.CreatedDate
        };
    }
}