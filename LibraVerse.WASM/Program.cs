using LibraVerse.WASM;
using MudBlazor.Services;
using Microsoft.JSInterop;
using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var services = builder.Services;

var rootComponents = builder.RootComponents;

rootComponents.Add<App>("#app");

rootComponents.Add<HeadOutlet>("head::after");

services.AddSingleton<NotificationService>();

services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationProviderStateService>();

builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthenticationProviderStateService>());

services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 1000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
});

services.AddScoped<LibraVerseApiClient>(sp =>
{
    var jsRuntime = sp.GetRequiredService<IJSRuntime>();
    
    var handler = new JwtAuthorizationService(jsRuntime)
    {
        InnerHandler = new HttpClientHandler()
    };
    
    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:7115")
    };
    return new LibraVerseApiClient("https://localhost:7115", httpClient);
});


await builder.Build().RunAsync();