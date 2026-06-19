using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailio.Services;


namespace Retailio.Pages.Settings.Roles
{
    public class IndexModel : PageModel
    {
        private readonly PermissionService _permService;

        public IndexModel(PermissionService permService)
        {
            _permService = permService;
        }

        public bool CanManageRoles { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToPage("/Account/Login");

            var isOwner = await _permService.IsOwnerOrAdminAsync();
            var perms = await _permService.GetUserPermissionsAsync();

            if (!isOwner && !perms.Contains(PermissionSlugs.ViewRoles))
                return RedirectToPage("/Index");

            CanManageRoles = isOwner || perms.Contains(PermissionSlugs.ManageRoles);
            return Page();
        }
    }
}

