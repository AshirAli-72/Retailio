using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using E_Invoice_system.Data;
using E_Invoice_system.Models;

namespace E_Invoice_system.Pages.Sale
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.CurrencyService _currencyService;

        public IndexModel(ApplicationDbContext context, Services.CurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        public IList<SaleDisplayItem> Sales { get; set; } = new List<SaleDisplayItem>();
        public IList<ReturnDisplayItem> Returns { get; set; } = new List<ReturnDisplayItem>();
        public IList<CreditDisplayItem> Credits { get; set; } = new List<CreditDisplayItem>();

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
            public string? InvNo { get; set; }
            public string? CustomerName { get; set; }
            public string? Date { get; set; }
            public int no_of_items { get; set; }
            public decimal total_qty { get; set; }
            public decimal Price { get; set; }
            public decimal TotalPrice { get; set; }
            public decimal PaidAmount { get; set; }
            public decimal RemainingAmount { get; set; }
            public string? Status { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await _currencyService.GetSymbolAsync();
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToPage("/Account/Login");

            try
            {
                var salesQuery = _context.SalesHeader.AsNoTracking()
                    .Where(s => s.payment_method != "Credit"
                        && (s.status == null || s.status != "Pending"));

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
                        no_of_items = _context.sales.Count(d => d.sale_id == s.id),
                        qty = 0,
                        total_qty = _context.sales.Where(d => d.sale_id == s.id).Sum(d => d.qty),
                        Price = s.gross_total,
                        TotalPrice = s.net_payable,
                        CustomerName = _context.customers.Where(c => c.id == s.customer_id).Select(c => c.name).FirstOrDefault() ?? "Walk in",
                        PaymentMethod = s.payment_method,
                        Status = s.status,
                        IsReturned = false
                    })
                    .ToListAsync();

                Sales = salesList.ToList();

                var returnsQuery = _context.returns.AsNoTracking();
                ReturnTotalCount = await returnsQuery.CountAsync();
                ReturnTotalPages = (int)Math.Ceiling(ReturnTotalCount / (double)ReturnPageSize);
                if (ReturnPageNumber < 1) ReturnPageNumber = 1;
                if (ReturnTotalPages > 0 && ReturnPageNumber > ReturnTotalPages) ReturnPageNumber = ReturnTotalPages;

                Returns = await returnsQuery
                    .OrderByDescending(r => r.Id)
                    .Skip((ReturnPageNumber - 1) * ReturnPageSize)
                    .Take(ReturnPageSize)
                    .GroupJoin(
                        _context.products_services.AsNoTracking(),
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
                            payment_method = x.r.payment_method,
                            Status = x.r.status
                        })
                    .ToListAsync();

                var creditsQuery = _context.credits_details.AsNoTracking()
                    .Where(c => c.status == null || c.status != "Returned");
                CreditTotalCount = await creditsQuery.CountAsync();
                CreditTotalPages = (int)Math.Ceiling(CreditTotalCount / (double)CreditPageSize);
                if (CreditPageNumber < 1) CreditPageNumber = 1;
                if (CreditTotalPages > 0 && CreditPageNumber > CreditTotalPages) CreditPageNumber = CreditTotalPages;

                Credits = await creditsQuery
                    .OrderByDescending(c => c.id)
                    .Skip((CreditPageNumber - 1) * CreditPageSize)
                    .Take(CreditPageSize)
                    .GroupJoin(
                        _context.credits.AsNoTracking(),
                        c => c.credit_id,
                        cr => cr.id,
                        (c, crs) => new { c, crs }
                    )
                    .SelectMany(
                        x => x.crs.DefaultIfEmpty(),
                        (x, cr) => new { x.c, cr }
                    )
                    .GroupJoin(
                        _context.customers.AsNoTracking(),
                        x => x.cr != null ? x.cr.customer_id : 0,
                        cust => cust.id,
                        (x, custs) => new { x.c, x.cr, custs }
                    )
                    .SelectMany(
                        x => x.custs.DefaultIfEmpty(),
                        (x, cust) => new CreditDisplayItem
                        {
                            id = x.c.id,
                            InvNo = x.cr != null ? x.cr.inv_no : "",
                            CustomerName = cust != null ? cust.name : "Walk in",
                            Date = x.c.date,
                            no_of_items = x.c.no_of_items,
                            total_qty = x.c.total_qty,
                            Price = x.c.price,
                            TotalPrice = x.c.total_price,
                            PaidAmount = x.c.paid_amount,
                            RemainingAmount = x.c.remaining_amount,
                            Status = x.c.status ?? "Pending"
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
