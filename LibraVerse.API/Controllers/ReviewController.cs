using LibraVerse.Models;
using LibraVerse.Attribute;
using LibraVerse.Persistence;
using LibraVerse.DTOs.Reviews;
using Microsoft.AspNetCore.Mvc;
using LibraVerse.Controllers.Base;
using LibraVerse.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraVerse.Controllers;

[Authorize]
public class ReviewController(ApplicationDbContext applicationDbContext, IUserService userService) : BaseController
{
    [HttpPost]
    [SwaggerOperation(OperationId = "CreateReview")]
    public Guid CreateReview(CreateReviewDto review)
    {
        var userId = userService.UserId;
        
        var user = applicationDbContext.Users.Find(userId)
            ?? throw new Exception("User not found.");
        
        var book = applicationDbContext.Books.Find(review.BookId)
            ?? throw new Exception("Book not found.");

        var orders = applicationDbContext.Orders
            .Where(x => x.UserId == user.Id)
            .Include(x => x.OrderDetails)
            .Where(x => x.OrderDetails.Any(z => z.BookId == review.BookId))
            .AsQueryable();
        
        if (!orders.Any())
            throw new Exception("You must purchase the book before reviewing it.");
        
        var reviewModel = new Review
        {
            BookId = book.Id,
            UserId = user.Id,
            Rating = review.Rating,
            Text = review.Text
        };
        
        applicationDbContext.Reviews.Add(reviewModel);
        
        applicationDbContext.SaveChanges();
        
        return reviewModel.Id;
    }
    
    [HttpPut("{reviewId:guid}")]
    [SwaggerOperation(OperationId = "UpdateReview")]
    public Guid UpdateReview(Guid reviewId, UpdateReviewDto review)
    {
        var reviewModel = applicationDbContext.Reviews.Find(reviewId)
                               ?? throw new Exception("Review not found.");
        
        reviewModel.Rating = review.Rating;
        reviewModel.Text = review.Text;
        
        applicationDbContext.Reviews.Update(reviewModel);
        applicationDbContext.SaveChanges();
        
        return reviewModel.Id;
    }
}