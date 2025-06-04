using MvcLibrary.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("LibraryDbContext")
        ?? throw new InvalidOperationException("Connection string 'LibraryDbContext' not found.")));

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Tworzenie domyślnego admina, jeśli brak użytkowników
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

    if (!db.Users.Any())
    {
        string hash;
        using (var sha = System.Security.Cryptography.SHA256.Create())
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes("admin123");
            var hashed = sha.ComputeHash(bytes);
            hash = BitConverter.ToString(hashed).Replace("-", "").ToLowerInvariant();
        }

        db.Users.Add(new MvcLibrary.Models.User
        {
            Login = "admin",
            PasswordHash = hash,
            IsAdmin = true,
            FirstName = "Admin",
            LastName = "Account"
        });

        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
