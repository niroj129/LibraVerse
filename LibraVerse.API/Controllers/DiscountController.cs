using LibraVerse.Models;
using LibraVerse.Attribute;
using LibraVerse.Persistence;
using Microsoft.AspNetCore.Mvc;
using LibraVerse.DTOs.Discount;
using LibraVerse.Controllers.Base;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraVerse.Controllers;

[Authorize]
public class DiscountController(ApplicationDbContext applicationDbContext) : BaseController
{
    [HttpPost]
    [SwaggerOperation(OperationId = "CreateDiscount")]
    public Guid CreateDiscount(CreateDiscountDto discount)
    {
        var book = applicationDbContext.Books.Find(discount.BookId)
            ?? throw new Exception("Book not found.");
        
        var existingDiscount = applicationDbContext.Discounts
            .FirstOrDefault(d => d.BookId == book.Id && d.StartDate.Date <= discount.EndDate.Date && d.EndDate.Date >= discount.StartDate.Date);
        
        if (existingDiscount != null)
        {
            throw new Exception("A discount already exists for this book during the specified date range.");
        }
        
        if (discount.StartDate >= discount.EndDate)
        {
            throw new Exception("Start date must be before end date.");
        }
        
        if (discount.DiscountPercentage is < 0 or > 100)
        {
            throw new Exception("Discount percentage must be between 0 and 100.");
        }
        
        if (discount.StartDate < DateTime.Now)
        {
            throw new Exception("Start date must be in the future.");
        }
        
        if (discount.EndDate < DateTime.Now)
        {
            throw new Exception("End date must be in the future.");
        }
        
        var discountModel = new Discount
        {
            BookId = book.Id,
            DiscountPercentage = discount.DiscountPercentage,
            MarkAsSale = discount.MarkAsSale,
            StartDate = discount.StartDate.Date,
            EndDate = discount.EndDate.Date
        };

        applicationDbContext.Discounts.Add(discountModel);
        applicationDbContext.SaveChanges();

        return discountModel.Id;
    }

    [HttpPut]
    [SwaggerOperation(OperationId = "UpdateDiscount")]
    public Guid UpdateDiscount(UpdateDiscountDto discount)
    {
        var discountModel = applicationDbContext.Discounts.Find(discount.Id)
            ?? throw new Exception("Discount not found.");
        
        discountModel.DiscountPercentage = discount.DiscountPercentage;
        discountModel.StartDate = discount.StartDate.Date;
        discountModel.EndDate = discount.EndDate.Date;
        discountModel.MarkAsSale = discount.MarkAsSale;

        applicationDbContext.Discounts.Update(discountModel);
        applicationDbContext.SaveChanges();
        
        return discountModel.Id;
    }
    
    [HttpDelete("{discountId:guid}")]
    [SwaggerOperation(OperationId = "DeleteDiscount")]
    public Guid DeleteDiscount(Guid discountId)
    {
        var discount = applicationDbContext.Discounts.Find(discountId)
            ?? throw new Exception("Discount not found.");
        
        applicationDbContext.Discounts.Remove(discount);
        applicationDbContext.SaveChanges();
        
        return discount.Id;
    }
}