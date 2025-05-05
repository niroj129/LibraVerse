using LibraVerse.Models;
using LibraVerse.Persistence;
using LibraVerse.DTOs.Books;
using LibraVerse.DTOs.Authors;
using Microsoft.EntityFrameworkCore;

namespace LibraVerse.Helper.Extension;

public static class BookExtensionMethod
{
    public static BookDto ToBookDto(this Book book, ApplicationDbContext dbContext, Guid? userId = null)
    {
        var dateTime = DateTime.Now;
        
        var discount = dbContext.Discounts.FirstOrDefault(x =>
            x.BookId == book.Id &&
            x.StartDate <= dateTime &&
            x.EndDate >= dateTime)?.ToDiscountDto();

        var discounts = dbContext.Discounts.Where(x => x.BookId == book.Id).ToList();
        
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            Genre = book.Genre.ToString(),
            Language = book.Language.ToString(),
            CoverImage = book.CoverImage,
            Publication = book.Publication?.ToPublicationDto() ?? new(),
            Format = book.Format?.ToFormatDto() ?? new(),
            Iban = book.Iban,
            IsAvailable = book.IsAvailable,
            Price = book.Price,
            PublishedDate = book.PublishedDate,
            Stock = book.Stock,
            IsAddedToWishlist = dbContext.Wishlists.Any(x => x.BookId == book.Id && x.UserId == userId),
            IsAddedToCart = dbContext.Carts.Any(x => x.BookId == book.Id && x.UserId == userId),
            IsPurchased = userId != null && dbContext.Orders.Where(x => x.UserId == userId).Include(x => x.OrderDetails).Any(x => x.OrderDetails.Any(z => z.BookId == book.Id)),
            Authors = book.BookAuthors.Select(x => x.Author?.ToAuthorDto() ?? new AuthorDto()).ToList(),
            Reviews = book.Reviews.Select(x => x.ToReviewDto()).ToList(),
            Discounts = discounts.Select(z => z.ToDiscountDto()).ToList(),
            Discount = discount,
        };
    }
}