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
                    string roleTitle = user.id == 1 ? "SuperAdmin" : "Owner";

                    HttpContext.Session.SetString("UserName",  user.username ?? user.email ?? "User");
                    HttpContext.Session.SetString("UserRole",  roleTitle);
                    HttpContext.Session.SetString("UserEmail", user.email ?? "");
                    HttpContext.Session.SetInt32("UserRoleId", user.role_id);
                    HttpContext.Session.SetInt32("UserId",     user.id);

                    TempData["Success"] = "Welcome back! Login successful.";

                    return PaymentHelper.HasAdminPanelAccess(roleTitle)
                        ? RedirectToPage("/Admin/AdminPanel")
                        : RedirectToPage("/Index");
                }

                // ── Check employee table for login ──────────────────────────
                var employee = await _context.employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.email == inputEmail);

                if (employee != null && !string.IsNullOrEmpty(employee.password)
                    && PaymentHelper.VerifyPassword(inputPass, employee.password))
                {
                    var employeeRole = employee.role_id.HasValue
                        ? await _context.roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == employee.role_id.Value)
                        : null;
                    string roleTitle = employeeRole?.RoleTitle ?? "Employee";

                    // Employee login — business_id is the admin who created this employee
                    HttpContext.Session.SetString("UserName",  employee.full_name ?? employee.email ?? "Employee");
                    HttpContext.Session.SetString("UserRole",  roleTitle);
                    HttpContext.Session.SetString("UserEmail", employee.email ?? "");
                    HttpContext.Session.SetInt32("UserRoleId", employee.role_id ?? 0);
                    HttpContext.Session.SetInt32("UserId",     employee.business_id ?? 0); // admin's business_id
                    HttpContext.Session.SetInt32("EmployeeId", employee.id);

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
