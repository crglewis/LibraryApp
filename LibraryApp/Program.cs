using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.SqlServer;
using LibraryApp.Data;
using Microsoft.Extensions.DependencyInjection;
using LibraryApp.Models;

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

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
     .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

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
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.Run();
