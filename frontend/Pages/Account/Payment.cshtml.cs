using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailio.Data;
using Retailio.Models;
using Retailio.Services;

namespace Retailio.Pages.Account
{
    public class PaymentModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PaymentModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string BusinessName { get; set; } = "";
        public string BusinessType { get; set; } = "";
        public string Plan { get; set; } = "";
        public string PlanPrice { get; set; } = "";

        public IActionResult OnGet()
        {
            Username = HttpContext.Session.GetString("Reg_Username") ?? "";
            Email = HttpContext.Session.GetString("Reg_Email") ?? "";
            BusinessName = HttpContext.Session.GetString("Reg_BusinessName") ?? "";
            BusinessType = HttpContext.Session.GetString("Reg_BusinessType") ?? "";
            Plan = HttpContext.Session.GetString("Reg_Plan") ?? "";

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Plan))
                return RedirectToPage("/Account/Register");

            PlanPrice = Plan switch
            {
                "professional" => "$49.00",
                "enterprise" => "$99.00",
                _ => "$0.00"
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var username = HttpContext.Session.GetString("Reg_Username") ?? "";
            var email = HttpContext.Session.GetString("Reg_Email") ?? "";
            var password = HttpContext.Session.GetString("Reg_Password") ?? "";
            var businessName = HttpContext.Session.GetString("Reg_BusinessName") ?? "";
            var businessType = HttpContext.Session.GetString("Reg_BusinessType") ?? "";
            var plan = HttpContext.Session.GetString("Reg_Plan") ?? "";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(plan))
                return RedirectToPage("/Account/Register");

            try
            {
                string startedAt = DateTime.UtcNow.ToString("yyyy-MM-dd");

                var newUser = new users
                {
                    username = username,
                    email = email,
                    password = PaymentHelper.HashPassword(password),
                    role_id = 1,
                    status = (int)EntityStatus.Active
                };
                _context.users.Add(newUser);
                await _context.SaveChangesAsync();

                var business = new Business
                {
                    user_id = newUser.id,
                    business_name = businessName,
                    business_type = businessType
                };
                _context.businesses.Add(business);

                var subscription = new Subscription
                {
                    business_id = newUser.id,
                    plan = plan,
                    started_at = startedAt,
                    expires_at = null,
                    status = (int)EntityStatus.Active
                };
                _context.subscriptions.Add(subscription);

                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("UserName", newUser.username ?? newUser.email ?? "User");
                HttpContext.Session.SetString("UserRole", "Owner");
                HttpContext.Session.SetString("UserEmail", newUser.email ?? "");
                HttpContext.Session.SetInt32("UserRoleId", newUser.role_id);
                HttpContext.Session.SetInt32("UserId", newUser.id);

                HttpContext.Session.Remove("Reg_Username");
                HttpContext.Session.Remove("Reg_Email");
                HttpContext.Session.Remove("Reg_Password");
                HttpContext.Session.Remove("Reg_BusinessName");
                HttpContext.Session.Remove("Reg_BusinessType");
                HttpContext.Session.Remove("Reg_Plan");

                TempData["Success"] = "Payment successful! Welcome to Retailio.";
                return RedirectToPage("/Index");
            }
            catch
            {
                return RedirectToPage("/Account/Register");
            }
        }
    }
}
