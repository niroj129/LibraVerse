using LibraVerse.Models;
using LibraVerse.DTOs.Discount;

namespace LibraVerse.Helper.Extension;

public static class DiscountExtensionMethod
{
    public static DiscountDto ToDiscountDto(this Discount discount)
    {
        return new DiscountDto
        {
            Id = discount.Id,
            MarkAsSale = discount.MarkAsSale,
            DiscountPercentage = discount.DiscountPercentage,
            StartDate = discount.StartDate,
            EndDate = discount.EndDate
        };
    }
}