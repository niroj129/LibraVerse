using Microsoft.AspNetCore.SignalR;
using LibraVerse.Services.Interface;

namespace LibraVerse.Notification;

public class NotificationHub(ConnectedUserTracker tracker, IUserService userService) : Hub
{
    public override Task OnConnectedAsync()
    {
        var userId = userService.UserId;
        
        tracker.AddUser(userId.ToString() ?? string.Empty, Context.ConnectionId);
        
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        tracker.RemoveConnection(Context.ConnectionId);
        
        return base.OnDisconnectedAsync(exception);
    }
}