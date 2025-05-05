using System.Text;
using Scalar.AspNetCore;
using LibraVerse.Services;
using LibraVerse.Settings;
using LibraVerse.Constants;
using LibraVerse.Middleware;
using LibraVerse.Persistence;
using LibraVerse.Notification;
using Microsoft.AspNetCore.Mvc;
using LibraVerse.Authentication;
using Microsoft.EntityFrameworkCore;
using LibraVerse.Services.Interface;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var configuration = builder.Configuration;

services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

services
    .AddAuthorization()
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.Authentication.ValueKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));

services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SupportNonNullableReferenceTypes();
});

services.AddSingleton<ConnectedUserTracker>();

services.AddSignalR();

services.AddScoped<IMailService, MailService>();

services.AddScoped<IUserService, UserService>();

services.AddHttpContextAccessor();

services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
});

services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5030", policy =>
    {
        policy.WithOrigins("https://localhost:5030")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.Title = "This is my Scalar API";
        options.DarkMode = true;
        options.Favicon = "path";
        options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.CSharp, ScalarClient.RestSharp);
        options.HideModels = false;
        options.Layout = ScalarLayout.Modern;
        options.ShowSidebar = true;

        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecurityScheme = "Bearer"
        };
    });
}

app.MapHub<NotificationHub>("/hubs/notifications");

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowLocalhost5030");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.UseSwaggerUI();

app.UseSwagger();

app.UseHsts();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    database.Database.Migrate();
    database.SeedAdmin();
}

app.Run();