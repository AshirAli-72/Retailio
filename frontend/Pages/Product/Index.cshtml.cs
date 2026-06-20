using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Retailio.Data;
using Retailio.Models;
using Retailio.Services;

namespace Retailio.Pages.Product
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.CurrencyService _currencyService;
        private readonly PermissionService _permService;

        public IndexModel(ApplicationDbContext context, Services.CurrencyService currencyService, PermissionService permService)
        {
            _context = context;
            _currencyService = currencyService;
            _permService = permService;
        }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public IList<ProductService> Products { get; set; } = default!;
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public int? UserId { get; set; }

        // ── Permission flags ──────────────────────────────────────
        public bool CanCreate { get; set; }
        public bool CanEdit   { get; set; }
        public bool CanDelete { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToPage("/Account/Login");

            UserName  = HttpContext.Session.GetString("UserName");
            UserEmail = HttpContext.Session.GetString("UserEmail");
            UserId    = HttpContext.Session.GetInt32("UserId");

            await _currencyService.GetSymbolAsync();

            // ── Permission check ──────────────────────────────────
            var isOwner = await _permService.IsOwnerOrAdminAsync();
            var perms   = await _permService.GetUserPermissionsAsync();

            if (!isOwner && !perms.Contains(PermissionSlugs.ViewProduct)
                        && !perms.Contains(PermissionSlugs.CreateProduct)
                        && !perms.Contains(PermissionSlugs.EditProduct)
                        && !perms.Contains(PermissionSlugs.DeleteProduct))
                return RedirectToPage("/Index");

            CanCreate = isOwner || perms.Contains(PermissionSlugs.CreateProduct);
            CanEdit   = isOwner || perms.Contains(PermissionSlugs.EditProduct);
            CanDelete = isOwner || perms.Contains(PermissionSlugs.DeleteProduct);

            IQueryable<ProductService> query = _context.products_services.AsNoTracking().ForTenant(UserId);
            TotalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            if (PageNumber < 1) PageNumber = 1;
            if (TotalPages > 0 && PageNumber > TotalPages) PageNumber = TotalPages;

            Products = await query.OrderByDescending(p => p.id)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var isOwner = await _permService.IsOwnerOrAdminAsync();
            var perms   = await _permService.GetUserPermissionsAsync();
            if (!isOwner && !perms.Contains(PermissionSlugs.DeleteProduct))
                return Forbid();

            var product = await _context.products_services.FindAsync(id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.pic))
                {
                    string storagePath = @"D:\netcore\Retailio\bin\Debug\images";
                    string filePath = Path.Combine(storagePath, product.pic);
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                _context.products_services.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Product deleted successfully.";
            }
            return RedirectToPage();
        }
    }
}
