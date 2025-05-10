using Microsoft.AspNetCore.SignalR.Client;

namespace LibraVerse.WASM.Services;

public class NotificationService
{
    private HubConnection? _hubConnection;

    public event Action<string>? OnNotificationReceived;

    public async Task StartConnection()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7115/hubs/notifications")
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string>("ReceiveNotification", (message) =>
        {
            OnNotificationReceived?.Invoke(message);
        });

        await _hubConnection.StartAsync();
    }

    public async Task StopConnection()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
        }
    }
}