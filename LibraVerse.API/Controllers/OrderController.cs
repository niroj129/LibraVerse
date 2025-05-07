using LibraVerse.Enums;
using LibraVerse.Models;
using LibraVerse.Attribute;
using LibraVerse.DTOs.Email;
using LibraVerse.DTOs.Order;
using LibraVerse.Persistence;
using LibraVerse.Notification;
using LibraVerse.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using LibraVerse.DTOs.Pagination;
using LibraVerse.Controllers.Base;
using LibraVerse.Helper.Extension;
using Microsoft.EntityFrameworkCore;
using LibraVerse.Services.Interface;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraVerse.Controllers;

[Authorize]
public class OrderController(ApplicationDbContext applicationDbContext, IUserService userService, IMailService mailService, IHubContext<NotificationHub> hubContext, ConnectedUserTracker connectedUserTracker) : BaseController
{
    [HttpGet]
    [SwaggerOperation(OperationId = "GetAllOrders")]
    public PaginatedResponse<OrderDto> GetAllOrders([FromQuery] PaginationQuery query, OrderStatus? status, Guid? userId)
    {
        var orders = applicationDbContext.Orders
            .Where(o => status == null || o.Status == status)
            .Where(o => userId == null || o.UserId == userId)
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Book)
            .Include(x => x.User)
            .ToList();
        
        var orderDetails = orders
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PaginatedResponse<OrderDto>
        {
            Items = orderDetails.Select(x => x.ToOrderDto(applicationDbContext)).ToList(),
            TotalCount = orders.Count(),
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
    
    [HttpGet("personal")]
    [SwaggerOperation(OperationId = "GetAllMyOrders")]
    public PaginatedResponse<OrderDto> GetAllMyOrders([FromQuery] PaginationQuery query, OrderStatus? status)
    {
        var userId = userService.UserId;
        
        var user = applicationDbContext.Users.Find(userId)
                   ?? throw new Exception("User not found.");
        
        var orders = applicationDbContext.Orders
            .Where(o => status == null || o.Status == status)
            .Where(o => o.UserId == user.Id)
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Book)
            .Include(x => x.User)
            .ToList();
        
        var orderDetails = orders
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PaginatedResponse<OrderDto>
        {
            Items = orderDetails.Select(x => x.ToOrderDto(applicationDbContext)).ToList(),
            TotalCount = orders.Count(),
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
    
    [HttpGet("list")]
    [SwaggerOperation(OperationId = "GetAllOrders")]
    public List<OrderDto> GetAllOrders(OrderStatus? status, Guid? userId)
    {
        var orders = applicationDbContext.Orders
            .Where(o => status == null || o.Status == status)
            .Where(o => userId == null || o.UserId == userId)
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Book)
            .Include(x => x.User)
            .ToList();
        
        return orders.Select(x => x.ToOrderDto(applicationDbContext)).ToList();
    }
    
    [HttpGet("personal/list")]
    [SwaggerOperation(OperationId = "GetAllMyOrdersList")]
    public List<OrderDto> GetAllMyOrders(OrderStatus? status)
    {
        var userId = userService.UserId;
        
        var user = applicationDbContext.Users.Find(userId)
                   ?? throw new Exception("User not found.");
        
        var orders = applicationDbContext.Orders
            .Where(o => o.UserId == user.Id)
            .Where(o => status == null || o.Status == status)
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Book)
            .Include(x => x.User)
            .ToList();
        
        return orders.Select(x => x.ToOrderDto(applicationDbContext)).ToList();
    }
    
    [HttpPost]
    [SwaggerOperation(OperationId = "CheckoutOrder")]
    public Guid CheckoutOrder()
    {
        var userId = userService.UserId;
        
        var user = applicationDbContext.Users.Find(userId)
            ?? throw new Exception("User not found.");
        
        var cart = applicationDbContext.Carts
            .Where(c => c.UserId == userId)
            .ToList();
        
        if (cart.Count == 0) throw new Exception("Cart is empty.");
        
        var previousUserOrders = applicationDbContext.Orders
            .Where(o => o.UserId == userId)
            .Include(x => x.OrderDetails)
            .ToList();
        
        var orderedBooks = previousUserOrders
            .SelectMany(o => o.OrderDetails)
            .Select(od => od.BookId)
            .ToList();

        var discountPercentage = previousUserOrders.Count > 10 
            ? 10 
            : orderedBooks.Count > 5 
                ? 5 
                : 0;

        var orderDetails = new List<OrderDetails>();
        
        foreach (var cartModel in cart)
        {
            var bookDiscount = applicationDbContext.Discounts
                .FirstOrDefault(d => d.BookId == cartModel.BookId && d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now);
            
            var book = applicationDbContext.Books
                .Find(cartModel.BookId)
                ?? throw new Exception("Book not found.");
            
            orderDetails.Add(new OrderDetails()
            {
                BookId = book.Id,
                Quantity = cartModel.Count,
                NetTotal = book.Price * cartModel.Count,
                BookDiscount = bookDiscount?.DiscountPercentage ?? 0,
                GrandTotal = bookDiscount != null 
                    ? (book.Price - book.Price * bookDiscount.DiscountPercentage / 100) * cartModel.Count 
                    : book.Price * cartModel.Count
            });
        }
        
        var order = new Order
        {
            UserId = user.Id,
            OrderDate = DateTime.Now,
            Status = OrderStatus.Pending,
            DiscountPercentage = discountPercentage,
            OrderDetails = orderDetails,
            TotalAmount = orderDetails.Sum(x => x.GrandTotal),
            GrandTotal = orderDetails.Sum(x => x.GrandTotal) - orderDetails.Sum(x => x.GrandTotal) * discountPercentage / 100
        };
        
        applicationDbContext.Orders.Add(order);
        applicationDbContext.Carts.RemoveRange(cart);
        
        applicationDbContext.SaveChanges();

        var orderCode = order.Id.ToString().Split('-')[0];
        var emailBody = $@"
            <div style='font-family: Arial, sans-serif; color: #333; padding: 20px;'>
                <h2 style='color: #2c3e50;'>📚 LibraVerse Order Confirmation</h2>
                <p>Hello <strong>{user.Name}</strong>,</p>
                <p>Thank you for your order! We've successfully received it and it's currently being processed.</p>
                <p><strong>Your Order Code:</strong> <span style='font-size: 18px; color: #2980b9;'>{orderCode}</span></p>
                <p>Keep this code handy. You'll need it to verify your order once it's delivered.</p>
                <br/>
                <p>Warm regards,<br/><strong>LibraVerse Team</strong></p>
                <hr style='margin-top: 30px;' />
                <p style='font-size: 12px; color: #888;'>This is an automated message. Please do not reply.</p>
            </div>
        ";

        var email = new EmailDto()
        {
            Body = emailBody,
            Subject = "📦 Your LibraVerse Order Confirmation.",
            ToEmail = user.Email,
            IsHtml = true
        };
        
        mailService.SendEmail(email);
        
        return order.Id;
    }
    
    [HttpPatch("{orderId:guid}")]
    [SwaggerOperation(OperationId = "UpdateOrder")]
    public Guid UpdateOrder(Guid orderId, UpdateOrderDto orderDto)
    {
        var order = applicationDbContext.Orders
                        .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Book)
                        .FirstOrDefault(x => x.Id == orderId)
            ?? throw new Exception("Order not found.");

        var orderIdentifier = order.Id.ToString().Split('-')[0];
        
        if (orderIdentifier != orderDto.Code)
            throw new Exception("Order code is not valid.");
        
        order.Status = OrderStatus.Completed;

        applicationDbContext.Orders.Update(order);

        foreach (var orderDetail in order.OrderDetails)
        {
            if (orderDetail.Book == null) continue;
            orderDetail.Book.Stock -= orderDetail.Quantity;
            applicationDbContext.Books.Update(orderDetail.Book);
        }
        
        applicationDbContext.SaveChanges();

        var userId = order.UserId.ToString();
        var connections = connectedUserTracker.GetConnectionIds(userId);
        
        foreach (var connectionId in connections)
        {
            hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", $"Order {order.Id} marked completed, thanks.");
        }
        
        hubContext.Clients.All.SendAsync("ReceiveNotification", "Congratulations, your order has been successfully completed.");
        
        return order.Id;
    }
    
    [HttpDelete("{orderId:guid}")]
    [SwaggerOperation(OperationId = "CancelOrder")]
    public Guid CancelOrder(Guid orderId)
    {
        var order = applicationDbContext.Orders.Find(orderId)
            ?? throw new Exception("Order not found.");
        
        if (order.Status == OrderStatus.Completed)
            throw new Exception("Order is already completed.");

        order.Status = OrderStatus.Cancelled;

        applicationDbContext.Orders.Update(order);
        applicationDbContext.SaveChanges();
        
        return order.Id;
    }
}