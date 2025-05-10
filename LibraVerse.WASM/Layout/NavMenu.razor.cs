using System.Security.Claims;
using LibraVerse.WASM.Services;
using Microsoft.JSInterop;

namespace LibraVerse.WASM.Layout;

public partial class NavMenu
{
    private string? _role;

    private bool _collapseNavMenu = true;

    private string? NavMenuCssClass => _collapseNavMenu ? "collapse" : null;

    private void ToggleNavMenu()
    {
        _collapseNavMenu = !_collapseNavMenu;
    }
    
    protected override async Task OnInitializedAsync()
    {
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        
        _role  = authenticationState.User.FindFirst(c => c.Type == ClaimTypes.Role)?.Value;
    }
    
    private async Task Logout()
    {
        await JsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");

        if (AuthenticationStateProvider is AuthenticationProviderStateService customAuth)
        {
            customAuth.NotifyUserLogout();
        }

        NavigationManager.NavigateTo("/", true);
    }
}