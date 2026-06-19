using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Retailio.Data;
using Retailio.Models;
using Retailio.Services;

namespace Retailio.Pages.Sale
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

        public IList<SaleDisplayItem> Sales { get; set; } = new List<SaleDisplayItem>();
        public IList<ReturnDisplayItem> Returns { get; set; } = new List<ReturnDisplayItem>();
        public IList<CreditDisplayItem> Credits { get; set; } = new List<CreditDisplayItem>();
        public IList<CreditTableDisplayItem> CreditTableItems { get; set; } = new List<CreditTableDisplayItem>();

        [BindProperty(SupportsGet = true)]
        public string CreditSubTab { get; set; } = "details";

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        [BindProperty(SupportsGet = true)]
        public int ReturnPageNumber { get; set; } = 1;
        public int ReturnPageSize { get; set; } = 10;
        public int ReturnTotalPages { get; set; }
        public int ReturnTotalCount { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CreditPageNumber { get; set; } = 1;
        public int CreditPageSize { get; set; } = 10;
        public int CreditTotalPages { get; set; }
        public int CreditTotalCount { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Tab { get; set; } = "sales";

        public string? ErrorMessage { get; set; }

        // ── Permission flags ──────────────────────────────────────
        public bool CanCreate { get; set; }
        public bool CanEdit   { get; set; }
        public bool CanDelete { get; set; }

        public class SaleDisplayItem
        {
            public int id { get; set; }
            public string? InvNo { get; set; }
            public string? Date { get; set; }
            public int no_of_items { get; set; }
            public decimal qty { get; set; }
            public decimal total_qty { get; set; }
            public decimal Price { get; set; }
            public decimal TotalPrice { get; set; }
            public string? CustomerName { get; set; }
            public string? PaymentMethod { get; set; }
            public string? Status { get; set; }
            public bool IsReturned { get; set; }
        }

        public class ReturnDisplayItem
        {
            public int Id { get; set; }
            public string? InvNo { get; set; }
            public string? ItemName { get; set; }
            public string? Date { get; set; }
            public int qty { get; set; }
            public decimal total_price { get; set; }
            public string? payment_method { get; set; }
            public string? Status { get; set; }
        }

        public class CreditDisplayItem
        {
            public int id { get; set; }
            public int CreditId { get; set; }
            public int CustomerId { get; set; }
            public int SaleId { get; set; }
            public string? CustomerName { get; set; }
            public string? Date { get; set; }
            public decimal amount { get; set; }
            public decimal CreditLimit { get; set; }
            public decimal UsedCredit { get; set; }
            public string? Status { get; set; }
        }

        public class CreditTableDisplayItem
        {
            public int id { get; set; }
            public string? CustomerName { get; set; }
            public string? Date { get; set; }
            public decimal total_credit { get; set; }
            public decimal paid { get; set; }
            public decimal remaining { get; set; }
            public decimal CreditLimit { get; set; }
            public string? Status { get; set; }
            public string? File { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await _currencyService.GetSymbolAsync();
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToPage("/Account/Login");

            var userId = HttpContext.Session.GetInt32("UserId");

            // ── Permission check ──────────────────────────────────
            var isOwner = await _permService.IsOwnerOrAdminAsync();
            var perms   = await _permService.GetUserPermissionsAsync();

            // Sales has no standalone view_ slug — any action permission grants access
            if (!isOwner && !PermissionSlugs.HasAnySale(perms))
                return RedirectToPage("/Index");

            CanCreate = isOwner || perms.Contains(PermissionSlugs.CreateSale);
            CanEdit   = isOwner || perms.Contains(PermissionSlugs.EditSale);
            CanDelete = isOwner || perms.Contains(PermissionSlugs.DeleteSale);

            try
            {
                var salesQuery = _context.SalesHeader.AsNoTracking().ForTenant(userId)
                    .Where(s => s.payment_method != (int?)PaymentMethod.Credit
                        && (s.status == null || s.status != (int?)PaymentStatus.Pending));

                TotalCount = await salesQuery.CountAsync();
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                if (PageNumber < 1) PageNumber = 1;
                if (TotalPages > 0 && PageNumber > TotalPages) PageNumber = TotalPages;

                var salesList = await salesQuery
                    .OrderByDescending(s => s.id)
                    .Skip((PageNumber - 1) * PageSize)
                    .Take(PageSize)
                    .Select(s => new SaleDisplayItem
                    {
                        id = s.id,
                        InvNo = s.inv_no ?? ("inv" + s.id.ToString("D3")),
                        Date = s.date,
                        no_of_items = _context.sales.ForTenant(userId).Count(d => d.sale_id == s.id),
                        qty = _context.sales.ForTenant(userId).Where(d => d.sale_id == s.id).Sum(d => d.qty),
                        total_qty = _context.sales.ForTenant(userId).Where(d => d.sale_id == s.id).Sum(d => d.qty),
                        Price = s.gross_total,
                        TotalPrice = s.net_payable,
                        CustomerName = _context.customers.ForTenant(userId).Where(c => c.id == s.customer_id).Select(c => c.name).FirstOrDefault() ?? "Walk in",
                        PaymentMethod = s.payment_method.HasValue ? PaymentHelper.GetPaymentMethodName(s.payment_method.Value) : null,
                        Status = s.status.HasValue ? PaymentHelper.GetStatusName(s.status.Value) : null,
                        IsReturned = false
                    })
                    .ToListAsync();

                Sales = salesList.ToList();

                var returnsQuery = _context.returns.AsNoTracking().ForTenant(userId);
                ReturnTotalCount = await returnsQuery.CountAsync();
                ReturnTotalPages = (int)Math.Ceiling(ReturnTotalCount / (double)ReturnPageSize);
                if (ReturnPageNumber < 1) ReturnPageNumber = 1;
                if (ReturnTotalPages > 0 && ReturnPageNumber > ReturnTotalPages) ReturnPageNumber = ReturnTotalPages;

                Returns = await returnsQuery
                    .OrderByDescending(r => r.Id)
                    .Skip((ReturnPageNumber - 1) * ReturnPageSize)
                    .Take(ReturnPageSize)
                    .GroupJoin(
                        _context.products_services.AsNoTracking().ForTenant(userId),
                        r => r.item_id,
                        p => p.id,
                        (r, prods) => new { r, prods }
                    )
                    .SelectMany(
                        x => x.prods.DefaultIfEmpty(),
                        (x, p) => new ReturnDisplayItem
                        {
                            Id = x.r.Id,
                            InvNo = x.r.inv_no,
                            ItemName = p != null ? p.prod_name : "Item #" + x.r.item_id,
                            Date = x.r.date,
                            qty = x.r.qty,
                            total_price = x.r.total_price,
                            payment_method = x.r.payment_method.HasValue ? PaymentHelper.GetPaymentMethodName(x.r.payment_method.Value) : null,
                            Status = x.r.status.HasValue ? PaymentHelper.GetStatusName(x.r.status.Value) : null
                        })
                    .ToListAsync();

                var creditTableQuery = _context.credits.AsNoTracking().ForTenant(userId);
                CreditTotalCount = await creditTableQuery.CountAsync();
                CreditTotalPages = (int)Math.Ceiling(CreditTotalCount / (double)CreditPageSize);
                if (CreditPageNumber < 1) CreditPageNumber = 1;
                if (CreditTotalPages > 0 && CreditPageNumber > CreditTotalPages) CreditPageNumber = CreditTotalPages;

                CreditTableItems = await creditTableQuery
                    .OrderByDescending(c => c.id)
                    .Skip((CreditPageNumber - 1) * CreditPageSize)
                    .Take(CreditPageSize)
                    .GroupJoin(
                        _context.customers.AsNoTracking().ForTenant(userId),
                        c => c.customer_id,
                        cust => cust.id,
                        (c, custs) => new { c, custs }
                    )
                    .SelectMany(
                        x => x.custs.DefaultIfEmpty(),
                        (x, cust) => new CreditTableDisplayItem
                        {
                            id = x.c.id,
                            CustomerName = cust != null ? cust.name : "Walk in",
                            Date = x.c.date,
                            total_credit = x.c.total_credit,
                            paid = x.c.paid,
                            remaining = x.c.remaining,
                            CreditLimit = cust != null ? (cust.credit_limit ?? 0) : 0,
                            Status = x.c.status.HasValue ? PaymentHelper.GetStatusName(x.c.status.Value) : "Pending",
                            File = x.c.file
                        })
                    .ToListAsync();
            }
            catch (Exception)
            {
                ErrorMessage = "Unable to load transaction data. Please refresh the page.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var isOwner = await _permService.IsOwnerOrAdminAsync();
            var perms   = await _permService.GetUserPermissionsAsync();
            if (!isOwner && !perms.Contains(PermissionSlugs.DeleteSale))
                return Forbid();

            var sale = await _context.SalesHeader.FindAsync(id);
            if (sale != null)
            {
                var details = await _context.sales.Where(d => d.sale_id == id).ToListAsync();
                _context.sales.RemoveRange(details);
                _context.SalesHeader.Remove(sale);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { Tab = "sales", PageNumber, ReturnPageNumber, CreditPageNumber });
        }

        public async Task<IActionResult> OnPostDeleteReturnAsync(int id)
        {
            var isOwner = await _permService.IsOwnerOrAdminAsync();
            var perms   = await _permService.GetUserPermissionsAsync();
            if (!isOwner && !perms.Contains(PermissionSlugs.DeleteSale))
                return Forbid();

            var returnRecord = await _context.returns.FindAsync(id);
            if (returnRecord != null)
            {
                _context.returns.Remove(returnRecord);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { Tab = "returns", PageNumber, ReturnPageNumber, CreditPageNumber });
        }

        public async Task<IActionResult> OnPostDeleteCreditAsync(int id)
        {
            var isOwner = await _permService.IsOwnerOrAdminAsync();
            var perms   = await _permService.GetUserPermissionsAsync();
            if (!isOwner && !perms.Contains(PermissionSlugs.DeleteSale))
                return Forbid();

            var credit = await _context.credits_details.FindAsync(id);
            if (credit != null)
            {
                _context.credits_details.Remove(credit);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { Tab = "credits", PageNumber, ReturnPageNumber, CreditPageNumber });
        }
    }
}
