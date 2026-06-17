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
        [BindProperty] public string Contact        { get; set; } = default!;
        [BindProperty] public string Cnic           { get; set; } = default!;
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

                if (await _context.users.AnyAsync(u => u.name != null &&
                        u.name.ToLower() == usernameTrimmed.ToLower()))
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

            // ── Paid plans: redirect to payment page ─────────
            if (SelectedPlan != "free_trial")
            {
                HttpContext.Session.SetString("Reg_Username",        usernameTrimmed);
                HttpContext.Session.SetString("Reg_Email",           emailTrimmed);
                HttpContext.Session.SetString("Reg_Password",        Password.Trim());
                HttpContext.Session.SetString("Reg_BusinessName",    BusinessName.Trim());
                HttpContext.Session.SetString("Reg_BusinessType",    BusinessType.Trim());
                HttpContext.Session.SetString("Reg_Plan",            SelectedPlan);
                HttpContext.Session.SetString("Reg_Contact",         Contact.Trim());
                HttpContext.Session.SetString("Reg_Cnic",            Cnic.Trim());
                return RedirectToPage("/Account/Payment");
            }

            // ── Free trial: create everything now ─────────────
            try
            {
                string startedAt = DateTime.UtcNow.ToString("yyyy-MM-dd");
                string? expiresAt = DateTime.UtcNow.AddDays(14).ToString("yyyy-MM-dd");

                // ── 1. Create user (business_id = 0 is allowed — no FK) ─────
                var newUser = new users
                {
                    name        = usernameTrimmed,
                    email       = emailTrimmed,
                    contact     = Contact.Trim(),
                    cnic        = Cnic.Trim(),
                    password    = PaymentHelper.HashPassword(Password.Trim()),
                    business_id = 0,
                    status      = (int)EntityStatus.Active
                };
                _context.users.Add(newUser);
                await _context.SaveChangesAsync();

                // ── 2. Create business (linked to user) ──────────────────────
                var business = new Business
                {
                    user_id       = newUser.id,
                    business_name = BusinessName.Trim(),
                    business_type = BusinessType.Trim()
                };
                _context.businesses.Add(business);
                await _context.SaveChangesAsync();

                // ── 3. Subscription record (linked to business) ──────────────
                var subscription = new Subscription
                {
                    business_id = business.id,
                    plan        = SelectedPlan,
                    started_at  = startedAt,
                    expires_at  = expiresAt,
                    status      = (int)EntityStatus.Active
                };
                _context.subscriptions.Add(subscription);

                // Link user back to their business
                newUser.business_id = business.id;

                await _context.SaveChangesAsync();

                // ── Auto-login ─────────────────────────────────
                HttpContext.Session.SetString("UserName",  newUser.name ?? newUser.email ?? "User");
                HttpContext.Session.SetString("UserRole",  "Owner");
                HttpContext.Session.SetString("UserEmail", newUser.email ?? "");
                HttpContext.Session.SetInt32("UserRoleId", business.id);
                HttpContext.Session.SetInt32("UserId",     business.id);
                HttpContext.Session.SetInt32("UserAccountId", newUser.id);

                TempData["Success"] = "Welcome to Retailio! Your account is ready.";
                return RedirectToPage("/Index");
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
