using LibraVerse.Models;
using LibraVerse.DTOs.Reviews;

namespace LibraVerse.Helper.Extension;

public static class ReviewExtensionMethod
{
    public static ReviewDto ToReviewDto(this Review review)
    {
        return new ReviewDto()
        {
            Id = review.Id,
            Rating = review.Rating,
            Text = review.Text,
            User = review.User?.ToUserDto() ?? new()
        };
    } 
}