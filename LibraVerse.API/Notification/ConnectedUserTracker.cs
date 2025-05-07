namespace LibraVerse.Notification;

public class ConnectedUserTracker
{
    private readonly Dictionary<string, string> _userConnections = new();

    public void AddUser(string userId, string connectionId)
    {
        lock (_userConnections)
        {
            _userConnections[connectionId] = userId;
        }
    }

    public void RemoveConnection(string connectionId)
    {
        lock (_userConnections)
        {
            _userConnections.Remove(connectionId);
        }
    }

    public IEnumerable<string> GetConnectionIds(string userId)
    {
        lock (_userConnections)
        {
            return _userConnections
                .Where(pair => pair.Value == userId)
                .Select(pair => pair.Key)
                .ToList();
        }
    }
}
