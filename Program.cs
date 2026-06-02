using E_Invoice_system.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();

// Required for session to work reliably
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(3),
                errorNumbersToAdd: new List<int> { 0, -2, 4060, 40197, 40501, 40613, 49918, 49919, 49920 });
            sqlOptions.CommandTimeout(60);
        }));

// Bridge for Razor Pages and other services that inject ApplicationDbContext directly
builder.Services.AddScoped<ApplicationDbContext>(p => 
    p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

builder.Services.AddScoped<E_Invoice_system.Services.CurrencyService>();


builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.MimeTypes = Microsoft.AspNetCore.ResponseCompression.ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

var app = builder.Build();



if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseResponseCompression(); 

// Serve images from the specific debug folder as requested by the user
string contentRoot = app.Environment.ContentRootPath;
string imagesDir = Path.Combine(contentRoot, "bin", "Debug", "images");
Console.WriteLine($"[DEBUG] Serving product images from: {imagesDir}");
string logoDir = Path.Combine(contentRoot, "bin", "Debug", "Logo");
string empImagesDir = Path.Combine(contentRoot, "bin", "Debug", "emp_image");

if (!Directory.Exists(imagesDir)) Directory.CreateDirectory(imagesDir);
if (!Directory.Exists(logoDir)) Directory.CreateDirectory(logoDir);
if (!Directory.Exists(empImagesDir)) Directory.CreateDirectory(empImagesDir);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(imagesDir),
    RequestPath = "/product-images"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(logoDir),
    RequestPath = "/store-logo"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(empImagesDir),
    RequestPath = "/emp-image"
});

app.UseStaticFiles(); // Default for wwwroot



app.UseRouting();


app.UseSession();

// app.UseAuthorization(); removed duplicate or check order

app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();

app.Run();

