// Namespace for notification-related components in the LibraVerse application
namespace LibraVerse.Notification;

/// <summary>
/// Maintains a thread-safe registry of active user connections for real-time notifications.
/// This singleton service tracks which users have active SignalR connections and allows
/// the application to find all connection IDs for a specific user.
/// </summary>
public class ConnectedUserTracker
{
    // Dictionary mapping connection IDs to user IDs
    // Key: SignalR connection ID (unique per client connection)
    // Value: User ID (can have multiple connections per user)
    private readonly Dictionary<string, string> _userConnections = new();

    /// <summary>
    /// Associates a user ID with a specific connection ID.
    /// This allows the application to send notifications to specific users
    /// across all their active devices/connections.
    /// </summary>
    /// <param name="userId">The ID of the user who established the connection</param>
    /// <param name="connectionId">The unique SignalR connection ID</param>
    public void AddUser(string userId, string connectionId)
    {
        // Use lock to ensure thread safety when modifying the dictionary
        // This prevents race conditions when multiple users connect simultaneously
        lock (_userConnections)
        {
            _userConnections[connectionId] = userId;
        }
    }

    /// <summary>
    /// Removes a connection when a client disconnects.
    /// Called when a user closes their browser or loses connection.
    /// </summary>
    /// <param name="connectionId">The SignalR connection ID to remove</param>
    public void RemoveConnection(string connectionId)
    {
        // Use lock to ensure thread safety when modifying the dictionary
        lock (_userConnections)
        {
            _userConnections.Remove(connectionId);
        }
    }

    /// <summary>
    /// Retrieves all active connection IDs for a specific user.
    /// Allows the application to broadcast notifications to all of a user's devices.
    /// </summary>
    /// <param name="userId">The ID of the user to find connections for</param>
    /// <returns>A list of connection IDs associated with the specified user</returns>
    public IEnumerable<string> GetConnectionIds(string userId)
    {
        // Use lock to ensure thread safety when reading from the dictionary
        lock (_userConnections)
        {
            // Find all connections for this user and convert to a list
            // to avoid enumeration issues outside the lock
            return _userConnections
                .Where(pair => pair.Value == userId)
                .Select(pair => pair.Key)
                .ToList();
        }
    }
}