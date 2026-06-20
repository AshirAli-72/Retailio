using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Retailio.Services;

namespace Retailio.Pages.Employee
{
    public class IndexModel : PageModel
    {
        private readonly PermissionService _permService;

        public IndexModel(PermissionService permService)
        {
            _permService = permService;
        }

        public string? UserName { get; set; }
        public int? UserId { get; set; }

        // ── Permission flags ──────────────────────────────────────
        public bool CanCreate { get; set; }
        public bool CanEdit   { get; set; }
        public bool CanDelete { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToPage("/Account/Login");

            UserName = HttpContext.Session.GetString("UserName");
            UserId   = HttpContext.Session.GetInt32("UserId");

            // ── Permission check ──────────────────────────────────
            var isOwner = await _permService.IsOwnerOrAdminAsync();
            var perms   = await _permService.GetUserPermissionsAsync();

            if (!isOwner && !perms.Contains(PermissionSlugs.ViewEmployee)
                        && !perms.Contains(PermissionSlugs.CreateEmployee)
                        && !perms.Contains(PermissionSlugs.EditEmployee)
                        && !perms.Contains(PermissionSlugs.DeleteEmployee))
                return RedirectToPage("/Index");

            CanCreate = isOwner || perms.Contains(PermissionSlugs.CreateEmployee);
            CanEdit   = isOwner || perms.Contains(PermissionSlugs.EditEmployee);
            CanDelete = isOwner || perms.Contains(PermissionSlugs.DeleteEmployee);

            return Page();
        }
    }
}
