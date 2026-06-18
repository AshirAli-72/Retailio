using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailio.Data;
using Retailio.Models;
using Retailio.Services;
using Microsoft.EntityFrameworkCore;

namespace Retailio.Pages.Customer
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

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public IList<customers> Customers { get; set; } = default!;
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
            var isOwner = _permService.IsOwnerOrAdmin();
            var perms   = await _permService.GetUserPermissionsAsync();

            if (!isOwner && !perms.Contains(PermissionSlugs.CreateCustomer)
                        && !perms.Contains(PermissionSlugs.EditCustomer)
                        && !perms.Contains(PermissionSlugs.DeleteCustomer))
                return RedirectToPage("/Index");

            CanCreate = isOwner || perms.Contains(PermissionSlugs.CreateCustomer);
            CanEdit   = isOwner || perms.Contains(PermissionSlugs.EditCustomer);
            CanDelete = isOwner || perms.Contains(PermissionSlugs.DeleteCustomer);

            IQueryable<customers> query = _context.customers.AsNoTracking().ForTenant(UserId);
            TotalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            if (PageNumber < 1) PageNumber = 1;
            if (TotalPages > 0 && PageNumber > TotalPages) PageNumber = TotalPages;

            Customers = await query.OrderByDescending(c => c.id)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Guard: only users with delete permission may delete
            var isOwner = _permService.IsOwnerOrAdmin();
            var perms   = await _permService.GetUserPermissionsAsync();
            if (!isOwner && !perms.Contains(PermissionSlugs.DeleteCustomer))
                return Forbid();

            var customer = await _context.customers.FindAsync(id);
            if (customer != null)
            {
                _context.customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
