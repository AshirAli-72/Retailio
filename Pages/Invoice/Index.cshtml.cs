using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using E_Invoice_system.Data;
using E_Invoice_system.Models;
using E_Invoice_system.Services;
using System.Text.Json;

namespace E_Invoice_system.Pages.Invoice
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly CurrencyService _currencyService;

        public IndexModel(ApplicationDbContext context, CurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public IList<InvoiceDisplayItem> Invoices { get; set; } = new List<InvoiceDisplayItem>();
        public string? ErrorMessage { get; set; }

        public class InvoiceDisplayItem
        {
            public int id { get; set; }
            public string? InvoiceNo { get; set; }
            public string? CustomerName { get; set; }
            public string? Date { get; set; }
            public string DisplayName { get; set; } = "";
            public string DisplayQty { get; set; } = "";
            public string DisplayExpiry { get; set; } = "";
            public decimal Discount { get; set; }
            public decimal TotalPrice { get; set; }
            public string? Payment { get; set; }
            public string? Status { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToPage("/Account/Login");

            await _currencyService.GetSymbolAsync();

            try
            {
                IQueryable<invoices> query = _context.invoices.AsNoTracking();
                TotalCount = await query.CountAsync();
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                
                if (PageNumber < 1) PageNumber = 1;
                if (TotalPages > 0 && PageNumber > TotalPages) PageNumber = TotalPages;

                var rawInvoices = await query
                    .OrderByDescending(i => i.date)
                    .Skip((PageNumber - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();

                Invoices = rawInvoices.Select(item => {
                    var display = new InvoiceDisplayItem
                    {
                        id = item.id,
                        InvoiceNo = item.invoice_no,
                        CustomerName = item.customer_name,
                        Date = item.date,
                        DisplayName = item.prod_name_service ?? "",
                        DisplayQty = item.qty_unit_type ?? "",
                        DisplayExpiry = "N/A",
                        Discount = item.discount,
                        TotalPrice = item.total_price,
                        Payment = item.payment,
                        Status = item.status
                    };

                    try {
                        if (!string.IsNullOrEmpty(display.DisplayName) && display.DisplayName.Trim().StartsWith("["))
                        {
                            var items = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object>>>(display.DisplayName);
                            if (items != null && items.Any())
                            {
                                var names = items.Select(i => i.ContainsKey("name") ? i["name"]?.ToString() : "").Where(n => !string.IsNullOrEmpty(n));
                                display.DisplayName = string.Join(", ", names);
                                
                                var qtys = items.Select(i => i.ContainsKey("qty") ? i["qty"]?.ToString() : "").Where(q => !string.IsNullOrEmpty(q));
                                display.DisplayQty = string.Join(", ", qtys);

                                var dates = items.Select(i => i.ContainsKey("expiryDate") ? i["expiryDate"]?.ToString() : null)
                                                 .Where(d => !string.IsNullOrEmpty(d))
                                                 .Select(d => d);
                                display.DisplayExpiry = string.Join(", ", dates);

                                // Truncate if too long
                                if (display.DisplayName.Length > 40) display.DisplayName = display.DisplayName.Substring(0, 37) + "...";
                                if (display.DisplayQty.Length > 20) display.DisplayQty = display.DisplayQty.Substring(0, 17) + "...";
                                if (display.DisplayExpiry.Length > 25) display.DisplayExpiry = display.DisplayExpiry.Substring(0, 22) + "...";
                            }
                        }
                    } catch { }

                    return display;
                }).ToList();
            }
            catch (Exception ex)
            {
                // Silence error for UI
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var invoice = await _context.invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.invoices.Remove(invoice);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Invoice deleted successfully.";
            }
            return RedirectToPage();
        }
    }
}
