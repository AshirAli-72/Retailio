using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailio.Data;
using Retailio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailio.Services;


namespace Retailio.Pages.Settings.User
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly PermissionService _permService;

        public IndexModel(ApplicationDbContext context, PermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public IList<users> Users { get; set; } = default!;

        [BindProperty]
        public users EditUser { get; set; } = new users();

        public bool CanCreateUser { get; set; }
        public bool CanEditUser { get; set; }
        public bool CanDeleteUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToPage("/Account/Login");

            var isOwner = _permService.IsOwnerOrAdmin();
            var perms = await _permService.GetUserPermissionsAsync();

            if (!isOwner && !perms.Contains(PermissionSlugs.ViewUsers))
                return RedirectToPage("/Index");

            CanCreateUser = isOwner || perms.Contains(PermissionSlugs.CreateUser);
            CanEditUser   = isOwner || perms.Contains(PermissionSlugs.EditUser);
            CanDeleteUser = isOwner || perms.Contains(PermissionSlugs.DeleteUser);

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
