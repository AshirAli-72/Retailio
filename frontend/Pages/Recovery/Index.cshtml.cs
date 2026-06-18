using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailio.Data;
using Retailio.Models;
using Microsoft.EntityFrameworkCore;
using Retailio.Services;

namespace Retailio.Pages.Recovery
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

        public string? UserName { get; set; }
        public int? UserId { get; set; }

        // ── Permission flags ──────────────────────────────────────
        public bool CanCreate { get; set; }
        public bool CanDelete { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToPage("/Account/Login");

            UserName = HttpContext.Session.GetString("UserName");
            UserId   = HttpContext.Session.GetInt32("UserId");

            // ── Permission check ──────────────────────────────────
            var isOwner = _permService.IsOwnerOrAdmin();
            var perms   = await _permService.GetUserPermissionsAsync();

            // Recovery has no standalone view_ slug — any action permission grants access
            if (!isOwner && !PermissionSlugs.HasAnyRecovery(perms))
                return RedirectToPage("/Index");

            CanCreate = isOwner || perms.Contains(PermissionSlugs.CreateRecovery);
            CanDelete = isOwner || perms.Contains(PermissionSlugs.DeleteRecovery);

            return Page();
        }
    }
}
