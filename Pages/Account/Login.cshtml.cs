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

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; } = default!;

        [BindProperty]
        public string Password { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return Page();
            }

            try
            {
                var inputEmail = Email.Trim();
                var inputPass = Password.Trim();

                // High-performance query: Single round-trip, no tracking
                var user = await _context.users
                    .Include(u => u.Role)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.email == inputEmail && u.password == inputPass);

                if (user != null)
                {
                    string roleTitle = user.Role?.RoleTitle ?? "User";

                    // Fast session storage
                    HttpContext.Session.SetString("UserName", user.email ?? "User");
                    HttpContext.Session.SetString("UserRole", roleTitle);
                    HttpContext.Session.SetString("UserEmail", user.email ?? "");
                    
                    TempData["Success"] = "Welcome back! Login successful.";
                    return RedirectToPage("/Index");
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Database connection error. Please try again.");
            }
            
            return Page();
        }
    }
 }

