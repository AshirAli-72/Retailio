using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailio.Services;

namespace Retailio.Pages.Reports
{
    public class IndexModel : PageModel
    {
        private readonly PermissionService _permService;

        public IndexModel(PermissionService permService)
        {
            _permService = permService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToPage("/Account/Login");
            }

            var isOwner = await _permService.IsOwnerOrAdminAsync();
            var perms   = await _permService.GetUserPermissionsAsync();

            if (!isOwner && !perms.Contains(PermissionSlugs.ViewReport))
                return RedirectToPage("/Index");

            return Page();
        }
    }
}
