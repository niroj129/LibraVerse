using MudBlazor;
using Microsoft.JSInterop;
using LibraVerse.WASM.Services;

namespace LibraVerse.WASM.Pages.Authentication;

public partial class Login
{
    private readonly LoginRequestDto _loginModel = new();
    
    private bool _isLoading;
    
    private async Task HandleLogin()
    {
        _isLoading = true;

        try
        {
            var result = await ApiClient.LoginAsync(_loginModel); 
            
            await JsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);

            AuthenticationStateService.NotifyUserAuthentication(result.Token);

            Snackbar.Add("Successfully Logged In", Severity.Success);
            
            NavigationManager.NavigateTo("/home");
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            _isLoading = false;
        }
    }
}