// Import necessary namespaces
using System.Text;                                 // For text encoding operations
using Scalar.AspNetCore;                           // Scalar API documentation tools
using LibraVerse.Services;                         // Custom service implementations
using LibraVerse.Settings;                         // Application settings classes
using LibraVerse.Constants;                        // Application constants
using LibraVerse.Middleware;                       // Custom middleware components
using LibraVerse.Persistence;                      // Database related components
using LibraVerse.Notification;                     // Real-time notification components
using Microsoft.AspNetCore.Mvc;                    // ASP.NET Core MVC framework
using LibraVerse.Authentication;                   // Authentication related components
using Microsoft.EntityFrameworkCore;               // Entity Framework Core for ORM
using LibraVerse.Services.Interface;               // Service interfaces
using Microsoft.IdentityModel.Tokens;              // JWT token handling
using Microsoft.AspNetCore.Mvc.Versioning;         // API versioning support
using Microsoft.AspNetCore.Mvc.ApplicationModels;  // MVC application models
using Microsoft.AspNetCore.Authentication.JwtBearer; // JWT bearer authentication

// Create the web application builder - entry point for configuration
var builder = WebApplication.CreateBuilder(args);

// Get the service collection for registering services
var services = builder.Services;

// Get the configuration for accessing app settings
var configuration = builder.Configuration;

// Add OpenAPI (Swagger) documentation with custom transformer for auth schemes
services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

// Configure authentication and authorization
services
    .AddAuthorization()
    .AddAuthentication(x =>
    {
        // Set JWT as the default authentication scheme
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        // Configure JWT bearer settings
        x.RequireHttpsMetadata = false;            // Don't require HTTPS for metadata
        x.SaveToken = true;                        // Save token after successful authentication
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = false,      // Don't validate signing key
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.Authentication.ValueKey)), // Set signing key
            ValidateIssuer = false,                // Don't validate token issuer
            ValidateAudience = false               // Don't validate token audience
        };
    });

// Add API versioning support
services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;  // Use default version if not specified
    options.DefaultApiVersion = new ApiVersion(1, 0);    // Set default API version to 1.0
    options.ReportApiVersions = true;                    // Include API versions in response headers
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Read version from URL segment
});

// Configure Entity Framework with SQLite database
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

// Register email settings from configuration
services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));

// Configure Swagger documentation generator
services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();                     // Enable Swagger annotations
    c.SupportNonNullableReferenceTypes();      // Add support for nullable reference types
});

// Register a singleton service for tracking connected users
services.AddSingleton<ConnectedUserTracker>();

// Add SignalR for real-time communications
services.AddSignalR();

// Register scoped services using dependency injection
services.AddScoped<IMailService, MailService>();    // Email service
services.AddScoped<IUserService, UserService>();    // User management service

// Add HTTP context accessor for accessing HTTP context in services
services.AddHttpContextAccessor();

// Add MVC controllers with custom route conventions
services.AddControllers(options =>
{
    // Use kebab-case for route parameters (e.g., user-profile instead of UserProfile)
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
});

// Configure CORS (Cross-Origin Resource Sharing) policy
services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5030", policy =>
    {
        policy.WithOrigins("https://localhost:5030") // Allow requests from this origin
            .AllowAnyHeader()                        // Allow any header
            .AllowAnyMethod();                       // Allow any HTTP method
    });
});

// Build the application
var app = builder.Build();

// Configure middleware pipeline based on environment
if (app.Environment.IsDevelopment())
{
    // Enable OpenAPI documentation in development environment
    app.MapOpenApi();

    // Configure Scalar API documentation
    app.MapScalarApiReference(options =>
    {
        options.Title = "This is my Scalar API";     // Documentation title
        options.DarkMode = true;                     // Enable dark mode
        options.Favicon = "path";                    // Set favicon path
        options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.CSharp, ScalarClient.RestSharp); // Default HTTP client
        options.HideModels = false;                  // Show models in documentation
        options.Layout = ScalarLayout.Modern;        // Use modern layout
        options.ShowSidebar = true;                  // Show sidebar

        // Configure authentication settings for documentation
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecurityScheme = "Bearer"       // Use Bearer token auth scheme
        };
    });
}

// Map SignalR hub for real-time notifications
app.MapHub<NotificationHub>("/hubs/notifications");

// Use custom middleware for global exception handling
app.UseMiddleware<ExceptionMiddleware>();

// Enable CORS with the defined policy
app.UseCors("AllowLocalhost5030");

// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

// Enable authentication middleware
app.UseAuthentication();

// Enable authorization middleware
app.UseAuthorization();

// Map controller routes
app.MapControllers();

// Serve static files (CSS, JS, images)
app.UseStaticFiles();

// Enable Swagger UI interface
app.UseSwaggerUI();

// Enable Swagger JSON endpoint
app.UseSwagger();

// Enable HTTP Strict Transport Security
app.UseHsts();

// Initialize database and seed admin user
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    database.Database.Migrate();      // Apply any pending migrations
    database.SeedAdmin();             // Seed admin user data
}

// Start the application
app.Run();