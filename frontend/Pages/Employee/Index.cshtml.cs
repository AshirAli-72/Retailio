using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace Retailio.Pages.Employee
{
    public class IndexModel : PageModel
    {
        public string? UserName { get; set; }
        public int? UserId { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToPage("/Account/Login");
            }
            UserName = HttpContext.Session.GetString("UserName");
            UserId = HttpContext.Session.GetInt32("UserId");
            return Page();
        }
    }
}
