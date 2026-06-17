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
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.email == inputEmail);

                if (user != null && PaymentHelper.VerifyPassword(inputPass, user.password))
                {
                    // Check if this user has role assignments (employee via user_has_roles)
                    var hasRoles = await _context.user_has_roles
                        .AnyAsync(e => e.UserId == user.id);

                    if (hasRoles)
                    {
                        // ── Employee login (has role assignments) ────────────
                        var rpIds = await _context.user_has_roles
                            .Where(e => e.UserId == user.id)
                            .Select(e => e.RolesHasPermissionId)
                            .ToListAsync();

                        string roleTitle = "Employee";
                        int roleId = 0;

                        if (rpIds.Any())
                        {
                            var roleIds = await _context.roles_has_permissions
                                .Where(rp => rpIds.Contains(rp.Id))
                                .Select(rp => rp.RoleId)
                                .Distinct()
                                .ToListAsync();

                            if (roleIds.Any())
                            {
                                var roleTitles = await _context.roles
                                    .Where(r => roleIds.Contains(r.Id))
                                    .Select(r => r.RoleTitle)
                                    .ToListAsync();

                                if (roleTitles.Any())
                                {
                                    roleTitle = string.Join(", ", roleTitles);
                                    roleId = roleIds.First();
                                }
                            }
                        }

                        // Find corresponding employee record
                        var employee = await _context.employees
                            .AsNoTracking()
                            .FirstOrDefaultAsync(e => e.email == inputEmail);

                        HttpContext.Session.SetString("UserName", user.name ?? user.email ?? "Employee");
                        HttpContext.Session.SetString("UserRole", roleTitle);
                        HttpContext.Session.SetString("UserEmail", user.email ?? "");
                        HttpContext.Session.SetInt32("UserRoleId", roleId);
                        HttpContext.Session.SetInt32("UserId", user.business_id);
                        HttpContext.Session.SetInt32("UserAccountId", user.id);
                        HttpContext.Session.SetInt32("EmployeeId", employee?.id ?? 0);

                        TempData["Success"] = "Welcome back! Login successful.";
                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        // ── Admin / Owner login (no role assignments — full access) ──
                        string roleTitle = user.id == 1 ? "SuperAdmin" : "Owner";

                        HttpContext.Session.SetString("UserName", user.name ?? user.email ?? "User");
                        HttpContext.Session.SetString("UserRole", roleTitle);
                        HttpContext.Session.SetString("UserEmail", user.email ?? "");
                        HttpContext.Session.SetInt32("UserRoleId", user.business_id);
                        HttpContext.Session.SetInt32("UserId", user.business_id);
                        HttpContext.Session.SetInt32("UserAccountId", user.id);

                        TempData["Success"] = "Welcome back! Login successful.";

                        return PaymentHelper.HasAdminPanelAccess(roleTitle)
                            ? RedirectToPage("/Admin/AdminPanel")
                            : RedirectToPage("/Index");
                    }
                }

                // ── Legacy: check employee table directly (employees without users record) ──
                var legacyEmp = await _context.employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.email == inputEmail);

                if (legacyEmp != null && !string.IsNullOrEmpty(legacyEmp.password)
                    && PaymentHelper.VerifyPassword(inputPass, legacyEmp.password))
                {
                    HttpContext.Session.SetString("UserName", legacyEmp.name ?? legacyEmp.email ?? "Employee");
                    HttpContext.Session.SetString("UserRole", "Employee");
                    HttpContext.Session.SetString("UserEmail", legacyEmp.email ?? "");
                    HttpContext.Session.SetInt32("UserRoleId", 0);
                    HttpContext.Session.SetInt32("UserId", legacyEmp.business_id ?? 0);
                    HttpContext.Session.SetInt32("EmployeeId", legacyEmp.id);

                    TempData["Success"] = "Welcome back! Login successful.";
                    return RedirectToPage("/Index");
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
