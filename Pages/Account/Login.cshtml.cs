using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using E_Invoice_system.Data;
using E_Invoice_system.Models;
using Microsoft.Extensions.Caching.Memory;

namespace E_Invoice_system.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public LoginModel(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [BindProperty]
        public string Email { get; set; } = default!;

        [BindProperty]
        public string Password { get; set; } = default!;

        public string LogoSrc { get; set; } = "/images/sata-logo.png";

        private async Task LoadLogoAsync()
        {
            // Check cache first for high performance
            if (_cache.TryGetValue("StoreLogoSrc", out string? cachedLogo) && !string.IsNullOrEmpty(cachedLogo))
            {
                LogoSrc = cachedLogo;
                return;
            }

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            try
            {
                var store = await _context.store_configurations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cts.Token);

                if (store != null && !string.IsNullOrEmpty(store.LogoPath))
                {
                    LogoSrc = "/store-logo/" + Path.GetFileName(store.LogoPath);
                    // Cache the result for 1 hour
                    _cache.Set("StoreLogoSrc", LogoSrc, TimeSpan.FromHours(1));
                }
            }
            catch
            {
                // DB unavailable / timeout — silently use default logo.
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToPage("/Index");
            }
            
            // Set default logo first for fast initial render
            LogoSrc = "/images/sata-logo.png";
            
            // Load custom logo asynchronously (non-blocking)
            await LoadLogoAsync();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                await LoadLogoAsync();
                return Page();
            }

            try
            {
                var inputEmail = Email.Trim();
                var inputPass = Password.Trim();

                // Optimized: Fetch user and role in a single database round-trip
                // Removed .ToLower() to allow index usage (assuming CI collation)
                var user = await _context.users
                    .Include(u => u.Role)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.email == inputEmail && u.password == inputPass);

                if (user != null)
                {
                    string roleTitle = user.Role?.RoleTitle ?? "User";

                    HttpContext.Session.SetString("UserName", user.email ?? "User");
                    HttpContext.Session.SetString("UserRole", roleTitle);
                    TempData["Success"] = "Welcome back! Login successful.";
                    return RedirectToPage("/Index");
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            catch (Exception ex) when (ex.Message.Contains("Timeout", StringComparison.OrdinalIgnoreCase) ||
                                         ex.Message.Contains("Connection", StringComparison.OrdinalIgnoreCase) ||
                                         ex.InnerException?.Message?.Contains("Timeout", StringComparison.OrdinalIgnoreCase) == true)
            {
                ModelState.AddModelError(string.Empty, "The server is temporarily busy. Please try again in a few seconds.");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Local database error. Please ensure SQLEXPRESS is running.");
            }
            
            await LoadLogoAsync();
            return Page();
        }
    }
}
