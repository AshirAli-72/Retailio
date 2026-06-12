using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailio.Data;
using Retailio.Models;
using Microsoft.EntityFrameworkCore;

namespace Retailio.Pages.Settings.User
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<users> Users { get; set; } = default!;

        [BindProperty]
        public users EditUser { get; set; } = new users();

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToPage("/Account/Login");
            }
            
            Users = await _context.users.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage();
            }

            if (EditUser.id == 0)
            {
                _context.users.Add(EditUser);
                TempData["Success"] = "User added successfully.";
            }
            else
            {
                _context.users.Update(EditUser);
                TempData["Success"] = "User updated successfully.";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var user = await _context.users.FindAsync(id);
            if (user != null)
            {
                _context.users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "User deleted successfully.";
            }
            return RedirectToPage();
        }
    }
}
