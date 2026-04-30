using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using E_Invoice_system.Data;
using E_Invoice_system.Models;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace E_Invoice_system.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly IMemoryCache _cache;
        private readonly Services.CurrencyService _currencyService;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context, IMemoryCache cache, Services.CurrencyService currencyService)
        {
            _logger = logger;
            _context = context;
            _cache = cache;
            _currencyService = currencyService;
        }

        public class DashboardStats
        {
            public int TotalInvoices { get; set; }
            public int TotalCustomers { get; set; }
            public int TotalProducts { get; set; }
            public decimal TotalSales { get; set; }
            public string[] StatusLabels { get; set; } = Array.Empty<string>();
            public int[] StatusCounts { get; set; } = Array.Empty<int>();
            public string[] TrendLabels { get; set; } = Array.Empty<string>();
            public int[] TrendData { get; set; } = Array.Empty<int>();
            public List<invoices> RecentInvoices { get; set; } = new List<invoices>();
            public int SaleCount { get; set; }
            public int ReturnCount { get; set; }
            public int LowStockCount { get; set; }
        }

        public IEnumerable<invoices> RecentInvoices { get; set; } = new List<invoices>();
        public int TotalInvoices { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public decimal TotalSales { get; set; }
        public string[] StatusLabels { get; set; } = Array.Empty<string>();
        public int[] StatusCounts { get; set; } = Array.Empty<int>();
        public string[] TrendLabels { get; set; } = Array.Empty<string>();
        public int[] TrendData { get; set; } = Array.Empty<int>();
        public int SaleCount { get; set; }
        public int ReturnCount { get; set; }
        public int LowStockCount { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToPage("/Account/Login");
            }

            await _currencyService.GetSymbolAsync();

            const string cacheKey = "Dashboard_Stats";
            if (_cache.TryGetValue(cacheKey, out DashboardStats? cachedStats) && cachedStats != null)
            {
                PopulateFromStats(cachedStats);
                return Page();
            }

            try
            {
                var sw = Stopwatch.StartNew();

                // Live Stats
                var stats = new DashboardStats();
                stats.TotalInvoices = await _context.invoices.AsNoTracking().CountAsync();
                stats.TotalCustomers = await _context.customers.AsNoTracking().CountAsync();
                stats.TotalProducts = await _context.products_services.AsNoTracking().CountAsync();
                stats.TotalSales = await _context.sales.AsNoTracking()
                    .Where(s => s.total_price > 0)
                    .SumAsync(s => (decimal?)s.total_price) ?? 0;

                var invoiceStatusData = await _context.invoices
                    .AsNoTracking()
                    .GroupBy(i => i.status ?? "Pending")
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();

                stats.StatusLabels = invoiceStatusData.Select(x => x.Status).ToArray();
                stats.StatusCounts = invoiceStatusData.Select(x => x.Count).ToArray();

                // Optimized Trend Query: Only fetch last 7 days from DB
                var startDate = DateTime.Today.AddDays(-6);
                var startDateStr = startDate.ToString("yyyy-MM-dd");

                var trendResultsFromDb = await _context.invoices
                    .AsNoTracking()
                    .Where(inv => inv.date != null && inv.date.CompareTo(startDateStr) >= 0)
                    .GroupBy(inv => inv.date)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .ToListAsync();

                var trendLabelsList = new List<string>();
                var trendDataList = new List<int>();

                for (int i = 6; i >= 0; i--)
                {
                    var targetDate = DateTime.Today.AddDays(-i);
                    var targetDateStr = targetDate.ToString("yyyy-MM-dd");
                    trendLabelsList.Add(targetDate.ToString("MMM dd"));
                    
                    var countEntry = trendResultsFromDb.FirstOrDefault(r => r.Date == targetDateStr);
                    trendDataList.Add(countEntry?.Count ?? 0);
                }

                stats.TrendLabels = trendLabelsList.ToArray();
                stats.TrendData = trendDataList.ToArray();

                // Recent Invoices (limit to 4)
                stats.RecentInvoices = await _context.invoices
                    .AsNoTracking()
                    .OrderByDescending(i => i.id)
                    .Take(4)
                    .ToListAsync();

                // Sale vs Return Counts
                stats.SaleCount = await _context.sales.AsNoTracking().CountAsync(s => !s.is_returned);
                stats.ReturnCount = await _context.returns.AsNoTracking().CountAsync();

                // Low Stock Count
                stats.LowStockCount = await _context.stock_details.AsNoTracking()
                    .CountAsync(s => s.quantity <= s.stock_alert);

                // Cache for 30 seconds
                _cache.Set(cacheKey, stats, TimeSpan.FromSeconds(30));
                PopulateFromStats(stats);

                _logger.LogInformation("Dashboard load completed in {ms}ms", sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard statistics.");
                ErrorMessage = "The database is temporarily busy. Please refresh the page in a few seconds.";
            }

            return Page();
        }

        private void PopulateFromStats(DashboardStats stats)
        {
            TotalInvoices = stats.TotalInvoices;
            TotalCustomers = stats.TotalCustomers;
            TotalProducts = stats.TotalProducts;
            TotalSales = stats.TotalSales;
            StatusLabels = stats.StatusLabels;
            StatusCounts = stats.StatusCounts;
            TrendLabels = stats.TrendLabels;
            TrendData = stats.TrendData;
            RecentInvoices = stats.RecentInvoices;
            SaleCount = stats.SaleCount;
            ReturnCount = stats.ReturnCount;
            LowStockCount = stats.LowStockCount;
        }
    }
}
