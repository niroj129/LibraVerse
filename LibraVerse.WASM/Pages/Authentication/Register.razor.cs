using LibraVerse.WASM.Services;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Authentication;

public partial class Register
{
    private bool _isLoading;
    private bool _termsAccepted;
    private string _errorMessage = string.Empty;
    private readonly RegisterRequestDto _registerModel = new();

    private async Task HandleRegistration()
    {
        if (!_termsAccepted)
        {
            _errorMessage = "You must accept the Terms of Service and Privacy Policy";
            return;
        }

        _isLoading = true;
        _errorMessage = string.Empty;

        try
        {
            await ApiClient.RegisterAsync(_registerModel);
            
            Snackbar.Add("Registration successful.", Severity.Success);

            NavigationManager.NavigateTo("/login");
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