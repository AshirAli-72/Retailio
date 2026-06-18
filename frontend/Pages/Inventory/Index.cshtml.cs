using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Retailio.Data;
using Retailio.Models;
using Retailio.Services;

namespace Retailio.Pages.Inventory
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

        public List<InventoryItem> Products { get; set; } = new();

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        // ── Permission flags ──────────────────────────────────────
        public bool CanEdit { get; set; }

        public class InventoryItem
        {
            public int id { get; set; }
            public string? prod_name { get; set; }
            public string? barcode { get; set; }
            public string? prod_state { get; set; }
            public string? unit { get; set; }
            public string? item_type { get; set; }
            public int? size { get; set; }
            public string? pic { get; set; }
            public string? status { get; set; }
            public int? category_id { get; set; }
            public int? brand_id { get; set; }
            public string CategoryName { get; set; } = "-";
            public string BrandName { get; set; } = "-";
        }

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToPage("/Account/Login");

            PageNumber = pageNumber;
            var userId = HttpContext.Session.GetInt32("UserId");

            // ── Permission check ──────────────────────────────────
            var isOwner = _permService.IsOwnerOrAdmin();
            var perms   = await _permService.GetUserPermissionsAsync();

            // Inventory has no standalone view_ slug — any action permission grants access
            if (!isOwner && !PermissionSlugs.HasAnyInventory(perms))
                return RedirectToPage("/Index");

            CanEdit = isOwner || perms.Contains(PermissionSlugs.EditInventory);

            IQueryable<ProductService> query = _context.products_services.AsNoTracking().ForTenant(userId);

            TotalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            var products = await query
                .OrderByDescending(p => p.id)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var categories = await _context.categories.AsNoTracking().ForTenant(userId).ToListAsync();
            var brands     = await _context.brands.AsNoTracking().ForTenant(userId).ToListAsync();

            Products = products.Select(p => new InventoryItem
            {
                id         = p.id,
                prod_name  = p.prod_name,
                barcode    = p.barcode,
                prod_state = p.prod_state,
                unit       = p.unit,
                item_type  = p.item_type,
                size       = p.size,
                pic        = p.pic,
                status     = PaymentHelper.GetEntityStatusName(p.status),
                category_id = p.category_id,
                brand_id   = p.brand_id,
                CategoryName = p.category_id != null ? categories.FirstOrDefault(c => c.id == p.category_id)?.category_title ?? "-" : "-",
                BrandName    = p.brand_id != null ? brands.FirstOrDefault(b => b.id == p.brand_id)?.brand_title ?? "-" : "-"
            }).ToList();

            return Page();
        }
    }
}
