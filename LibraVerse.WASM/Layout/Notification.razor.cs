using MudBlazor;

namespace LibraVerse.WASM.Layout;

public partial class Notification
{
    protected override async Task OnInitializedAsync()
    {
        NotificationService.OnNotificationReceived += ShowMessage;
        await NotificationService.StartConnection();
    }

    private void ShowMessage(string message)
    {
        Snackbar.Add(message, Severity.Success, c => c.SnackbarVariant = Variant.Outlined);
    }
}