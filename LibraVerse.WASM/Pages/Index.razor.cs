namespace LibraVerse.WASM.Pages;

public partial class Index
{
    protected override async Task OnInitializedAsync()
    {
        var authenticationState = await AuthenticationStateService.GetAuthenticationStateAsync();

        var isUserLoggedIn = authenticationState.User.Identity?.IsAuthenticated;

        if (isUserLoggedIn == false)
        {
            NavigationManager.NavigateTo("/login");
            return;
        }

        NavigationManager.NavigateTo("/home");
    }
}