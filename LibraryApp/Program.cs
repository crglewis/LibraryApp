using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LibraryApp.Data;
using Microsoft.Extensions.DependencyInjection;
using LibraryApp.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Read SQL credentials from environment variables (case-sensitive: sqluser, sqlpass)
string? sqluser = Environment.GetEnvironmentVariable("sqluser");
string? sqlpass = Environment.GetEnvironmentVariable("sqlpass");

if (string.IsNullOrEmpty(sqluser) || string.IsNullOrEmpty(sqlpass))
{
    throw new InvalidOperationException(
         $"SQL credentials not configured. Set the following environment variables: " +
         $"export sqluser='your_username' && export sqlpass='your_password'");
}

// Build connection string with hardcoded server/database and env vars for username/password
// Encryption disabled to avoid SSL certificate chain validation issues on local development
string connectionString = $"Server=.;Database=LibraryDB;User Id={sqluser};Password={sqlpass};Encrypt=False;TrustServerCertificate=False;";

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Relaxed to match the frontend's client-side validation (min 6 characters).
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// The Angular SPA talks to the API over fetch/XHR, not browser navigation, so unauthenticated/
// unauthorized requests should get plain status codes instead of a redirect to a login page.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Book <-> Review navigation properties can form a cycle; drop the back-reference
        // instead of throwing when EF has populated both sides.
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure migrations are applied and data is seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    context.SeedData();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
