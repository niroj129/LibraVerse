// Required namespaces for SignalR functionality and service dependencies
using Microsoft.AspNetCore.SignalR;      // Core SignalR functionality
using LibraVerse.Services.Interface;     // Application service interfaces

// Namespace for notification-related components in the LibraVerse application
namespace LibraVerse.Notification;

/// <summary>
/// SignalR hub for handling real-time notifications to connected clients.
/// This hub manages client connections and disconnections while integrating
/// with the application's user tracking system.
/// </summary>
/// <remarks>
/// Uses C# 12 primary constructor syntax to inject dependencies:
/// - ConnectedUserTracker: Keeps track of which users have active connections
/// - IUserService: Provides information about the authenticated user
/// </remarks>
public class NotificationHub(ConnectedUserTracker tracker, IUserService userService) : Hub
{
    /// <summary>
    /// Executes when a new client connects to the hub.
    /// Registers the user's connection in the tracker for future notifications.
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    public override Task OnConnectedAsync()
    {
        // Get current user ID from the user service
        var userId = userService.UserId;
        
        // Register the user connection in the tracker
        // If userId is null, store an empty string to avoid null reference issues
        tracker.AddUser(userId.ToString() ?? string.Empty, Context.ConnectionId);
        
        // Call the base implementation to complete the connection process
        return base.OnConnectedAsync();
    }

    /// <summary>
    /// Executes when a client disconnects from the hub.
    /// Removes the connection from the tracker to prevent sending notifications
    /// to disconnected clients.
    /// </summary>
    /// <param name="exception">Exception that caused the disconnection, if any</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        // Remove the connection from the tracker
        tracker.RemoveConnection(Context.ConnectionId);
        
        // Call the base implementation to complete the disconnection process
        return base.OnDisconnectedAsync(exception);
    }
}