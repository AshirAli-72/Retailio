using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Retailio.Data;
using Retailio.Models;
using Retailio.Services;

namespace Retailio.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegisterModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public string Username        { get; set; } = default!;
        [BindProperty] public string Email           { get; set; } = default!;
        [BindProperty] public string Password        { get; set; } = default!;
        [BindProperty] public string ConfirmPassword { get; set; } = default!;
        [BindProperty] public string BusinessName    { get; set; } = default!;
        [BindProperty] public string BusinessType    { get; set; } = default!;
        [BindProperty] public string SelectedPlan    { get; set; } = "free_trial";

        public IActionResult OnGet()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                var role = HttpContext.Session.GetString("UserRole") ?? "";
                return PaymentHelper.HasAdminPanelAccess(role)
                    ? RedirectToPage("/Admin/AdminPanel")
                    : RedirectToPage("/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // ── Validate plan ─────────────────────────────────
            var validPlans = new[] { "free_trial", "professional", "enterprise" };
            if (string.IsNullOrWhiteSpace(SelectedPlan) || !validPlans.Contains(SelectedPlan))
                SelectedPlan = "free_trial";

            // ── Field validation ──────────────────────────────
            if (string.IsNullOrWhiteSpace(Username))
            { ModelState.AddModelError(string.Empty, "Username is required."); return Page(); }

            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
            { ModelState.AddModelError(string.Empty, "A valid email address is required."); return Page(); }

            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            { ModelState.AddModelError(string.Empty, "Password must be at least 6 characters."); return Page(); }

            if (Password != ConfirmPassword)
            { ModelState.AddModelError(string.Empty, "Passwords do not match."); return Page(); }

            if (string.IsNullOrWhiteSpace(BusinessName))
            { ModelState.AddModelError(string.Empty, "Business name is required."); return Page(); }

            if (string.IsNullOrWhiteSpace(BusinessType))
            { ModelState.AddModelError(string.Empty, "Business type is required."); return Page(); }

            var emailTrimmed    = Email.Trim().ToLower();
            var usernameTrimmed = Username.Trim();

            // ── Duplicate email / username checks ─────────────
            try
            {
                // Case-insensitive email check
                if (await _context.users.AnyAsync(u => u.email != null &&
                        u.email.ToLower() == emailTrimmed))
                {
                    ModelState.AddModelError(string.Empty, "An account with this email already exists.");
                    return Page();
                }

                if (await _context.users.AnyAsync(u => u.username != null &&
                        u.username.ToLower() == usernameTrimmed.ToLower()))
                {
                    ModelState.AddModelError(string.Empty, "This username is already taken.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Registration error: " + (ex.InnerException?.Message ?? ex.Message));
                return Page();
            }

            // ── Calculate trial/subscription dates ───────────
            string startedAt  = DateTime.UtcNow.ToString("yyyy-MM-dd");
            string? expiresAt = SelectedPlan == "free_trial"
                ? DateTime.UtcNow.AddDays(14).ToString("yyyy-MM-dd")
                : null;   // paid plans: no hard expiry until payment confirmed

            // ── Create user, business, subscription ──────────
            try
            {
                // ── 1. Create Admin role first (user_id NULL temporarily) ───
                var userRole = new Role { RoleTitle = "Admin" };
                _context.roles.Add(userRole);
                await _context.SaveChangesAsync(); // get userRole.Id

                // ── 2. Create user with the new role_id ──────────────────────
                var newUser = new users
                {
                    username = usernameTrimmed,
                    email    = emailTrimmed,
                    password = PaymentHelper.HashPassword(Password.Trim()),
                    role_id  = userRole.Id,
                    status   = (int)EntityStatus.Active
                };
                _context.users.Add(newUser);
                await _context.SaveChangesAsync(); // get newUser.id

                // Admin accounts get full POS access via role title — no roles_permissions row.
                // roles_permissions is only for employee role restrictions.

                // ── 3. Business info ─────────────────────────────────────────
                var business = new Business
                {
                    user_id       = newUser.id,
                    business_name = BusinessName.Trim(),
                    business_type = BusinessType.Trim()
                };
                _context.businesses.Add(business);

                // ── 4. Subscription record ───────────────────────────────────
                var subscription = new Subscription
                {
                    user_id    = newUser.id,
                    plan       = SelectedPlan,
                    started_at = startedAt,
                    expires_at = expiresAt,
                    status     = (int)EntityStatus.Active
                };
                _context.subscriptions.Add(subscription);

                await _context.SaveChangesAsync();

                // ── Set success message based on plan ─────────
                TempData["Success"] = SelectedPlan == "free_trial"
                    ? "Account created! Your 14-day free trial has started. Sign in to continue."
                    : $"Account created with the {SelectedPlan} plan! Sign in to get started.";

                return RedirectToPage("/Account/Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty,
                    "Registration failed: " + (ex.InnerException?.Message ?? ex.Message));
                return Page();
            }
        }
    }
}
