// Import required namespaces
using LibraVerse.WASM;                                   // Main application namespace
using MudBlazor.Services;                                // MudBlazor UI component library services
using Microsoft.JSInterop;                               // JavaScript interoperability
using LibraVerse.WASM.Services;                          // Custom application services
using Microsoft.AspNetCore.Components.Authorization;      // Blazor authentication services
using Microsoft.AspNetCore.Components.Web;                // Web-specific Blazor components
using Microsoft.AspNetCore.Components.WebAssembly.Hosting; // WebAssembly hosting services
using MudBlazor;                                         // MudBlazor UI component library

// Create the WebAssembly host builder - entry point for WASM app configuration
var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Get the service collection for registering services
var services = builder.Services;

// Get the root components collection for registering top-level components
var rootComponents = builder.RootComponents;

// Register the main App component to render in the browser's DOM element with id="app"
rootComponents.Add<App>("#app");

// Register the HeadOutlet component to manage document head content (title, meta tags, etc.)
rootComponents.Add<HeadOutlet>("head::after");

// Register a singleton NotificationService to handle real-time notifications
// Using singleton ensures all components share the same notification instance
services.AddSingleton<NotificationService>();

// Add core authorization services for handling user permissions
services.AddAuthorizationCore();

// Register the custom authentication state provider as scoped service
// This manages the user's authentication state (login status, tokens, etc.)
builder.Services.AddScoped<AuthenticationProviderStateService>();

// Register the custom provider as the application's AuthenticationStateProvider
// This connects the custom provider to Blazor's authentication system
builder.Services.AddScoped<AuthenticationStateProvider>(sp => 
    sp.GetRequiredService<AuthenticationProviderStateService>());

// Register MudBlazor UI services with custom configuration
services.AddMudServices(config =>
{
    // Configure Snackbar notifications (toast messages)
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight; // Position in bottom-right corner
    config.SnackbarConfiguration.PreventDuplicates = false;                             // Allow duplicate notifications
    config.SnackbarConfiguration.NewestOnTop = false;                                   // Add new notifications at the bottom
    config.SnackbarConfiguration.ShowCloseIcon = true;                                  // Show close button on notifications
    config.SnackbarConfiguration.VisibleStateDuration = 1000;                           // Show notifications for 1 second
    config.SnackbarConfiguration.HideTransitionDuration = 500;                          // Fade-out animation duration
    config.SnackbarConfiguration.ShowTransitionDuration = 500;                          // Fade-in animation duration
    config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;                    // Use outlined visual style
});

// Register the API client as a scoped service with JWT authentication
services.AddScoped<LibraVerseApiClient>(sp =>
{
    // Get the JavaScript runtime for token storage access
    var jsRuntime = sp.GetRequiredService<IJSRuntime>();
    
    // Create a custom HTTP message handler that adds JWT tokens to requests
    var handler = new JwtAuthorizationService(jsRuntime)
    {
        InnerHandler = new HttpClientHandler() // Use standard handler for HTTP operations
    };
    
    // Create an HTTP client with the JWT handler and configure the base URL
    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:7115") // API endpoint base URL
    };

    // Create and return the typed API client with the configured HTTP client
    return new LibraVerseApiClient("https://localhost:7115", httpClient);
});

// Build and run the application
// The await keyword ensures we wait for the application to fully start
await builder.Build().RunAsync();