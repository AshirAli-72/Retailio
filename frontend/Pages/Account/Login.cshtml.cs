using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Retailio.Data;
using Retailio.Models;
using Retailio.Services;

namespace Retailio.Pages.Account
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

        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("UserRole") ?? "";

            // Already logged in — send to correct destination
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return PaymentHelper.HasAdminPanelAccess(role)
                    ? RedirectToPage("/Admin/AdminPanel")
                    : RedirectToPage("/Index");
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
                var inputPass  = Password.Trim();

                var user = await _context.users
                    .Include(u => u.Role)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.email == inputEmail);

                if (user != null && PaymentHelper.VerifyPassword(inputPass, user.password))
                {
                    // Use the role title directly from the included Role navigation property
                    string roleTitle = user.Role?.RoleTitle ?? "Unknown";

                    HttpContext.Session.SetString("UserName",  user.username ?? user.email ?? "User");
                    HttpContext.Session.SetString("UserRole",  roleTitle);
                    HttpContext.Session.SetString("UserEmail", user.email ?? "");
                    // Store numeric role_id for fast checks without string comparison
                    HttpContext.Session.SetInt32("UserRoleId", user.role_id);

                    TempData["Success"] = "Welcome back! Login successful.";

                    // SuperAdmin (role=1) → Admin panel
                    // Admin (role=2) and all others → POS dashboard
                    return PaymentHelper.HasAdminPanelAccess(roleTitle)
                        ? RedirectToPage("/Admin/AdminPanel")
                        : RedirectToPage("/Index");
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Login error: " + (ex.InnerException?.Message ?? ex.Message));
            }

            return Page();
        }
    }
}
