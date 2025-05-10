using Microsoft.JSInterop;
using System.Security.Claims;
using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LibraVerse.WASM.Layout;

public partial class MainLayout
{
    private bool _sidebarExpanded = true;
    private bool _isAuthenticated = false;
    private string? _userName = "John Doe";
    private string? _userEmail = "john.doe@example.com";
    
    [CascadingParameter]
    private Task<AuthenticationState>? authenticationState { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        if (authenticationState != null)
        {
            var authState = await authenticationState;
            var user = authState.User;
            
            if (user?.Identity?.IsAuthenticated ?? false)
            {
                _isAuthenticated = true;
                _userName = user.FindFirst(ClaimTypes.Name)?.Value ?? "User";
                _userEmail = user.FindFirst(ClaimTypes.Email)?.Value ?? "user@example.com";
            }
        }
    }
    
    private void ToggleSidebar()
    {
        _sidebarExpanded = !_sidebarExpanded;
    }
    
    private string GetPageTitle()
    {
        var uri = NavigationManager.Uri;
        var segment = uri.Split('/').LastOrDefault();
        
        if (string.IsNullOrEmpty(segment))
            return "Dashboard";
            
        return char.ToUpper(segment[0]) + segment.Substring(1);
    }
    
    private void NavigateToLogin()
    {
        NavigationManager.NavigateTo("/login");
    }
    
    private void ShowNotifications()
    {
        // Your notification logic
    }
    
    private void Logout()
    {
        // Your existing logout logic
    }
}