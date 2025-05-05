using LibraVerse.Models;
using LibraVerse.DTOs.Order;
using LibraVerse.Persistence;

namespace LibraVerse.Helper.Extension;

public static class OrderExtensionMethod
{
    public static OrderDto ToOrderDto(this Order order, ApplicationDbContext applicationDbContext)
    {
        return new OrderDto()
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            DiscountPercentage = order.DiscountPercentage,
            User = order.User?.ToUserDto() ?? new(),
            GrandTotal = order.GrandTotal,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            OrderDetails = order.OrderDetails.Select(orderDetails => new OrderDetailsDto()
            {
                Id = orderDetails.Id,
                Quantity = orderDetails.Quantity,
                GrandTotal = orderDetails.GrandTotal,
                Book = orderDetails.Book?.ToBookDto(applicationDbContext) ?? new(),
                BookDiscount = orderDetails.BookDiscount,
                NetTotal = orderDetails.NetTotal
            }).ToList()
        };
    }
}