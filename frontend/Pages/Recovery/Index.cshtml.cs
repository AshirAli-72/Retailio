using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailio.Data;
using Retailio.Models;
using Microsoft.EntityFrameworkCore;

namespace Retailio.Pages.Recovery
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string? UserName { get; set; }
        public int? UserId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToPage("/Account/Login");

            UserName = HttpContext.Session.GetString("UserName");
            UserId = HttpContext.Session.GetInt32("UserId");

            return Page();
        }
    }
}
