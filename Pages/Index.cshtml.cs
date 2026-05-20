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
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly ILogger<IndexModel> _logger;
        private readonly IMemoryCache _cache;
        private readonly Services.CurrencyService _currencyService;

        public IndexModel(ILogger<IndexModel> logger, IDbContextFactory<ApplicationDbContext> dbFactory, IMemoryCache cache, Services.CurrencyService currencyService)
        {
            _logger = logger;
            _dbFactory = dbFactory;
            _cache = cache;
            _currencyService = currencyService;
        }

        public class DashboardStats
        {
            public int TotalCustomers { get; set; }
            public int TotalProducts { get; set; }
            public decimal TotalSales { get; set; }
            public string[] TrendLabels { get; set; } = Array.Empty<string>();
            public int[] TrendData { get; set; } = Array.Empty<int>();
            public int SaleCount { get; set; }
            public int ReturnCount { get; set; }
            public int LowStockCount { get; set; }
        }

        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public decimal TotalSales { get; set; }
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

            const string cacheKey = "Dashboard_Stats_v2";
            if (_cache.TryGetValue(cacheKey, out DashboardStats? cachedStats) && cachedStats != null)
            {
                PopulateFromStats(cachedStats);
                return Page();
            }

            using var context = _dbFactory.CreateDbContext();
            
            // Heartbeat check
            try 
            { 
                if (!await context.Database.CanConnectAsync())
                {
                    _logger.LogError("Dashboard: Cannot connect to database.");
                    return Page();
                }
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, "Dashboard: Connection heartbeat failed.");
                return Page();
            }

            var stats = new DashboardStats();
            var sw = Stopwatch.StartNew();

            // 2. Total Customers
            try { stats.TotalCustomers = await context.customers.AsNoTracking().CountAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error fetching TotalCustomers"); }

            // 3. Total Products
            try { stats.TotalProducts = await context.products_services.AsNoTracking().CountAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error fetching TotalProducts"); }

            // 4. Total Sales
            try 
            { 
                stats.TotalSales = await context.sales.AsNoTracking()
                    .Where(s => s.total_price > 0)
                    .SumAsync(s => (decimal?)s.total_price) ?? 0; 
            }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error fetching TotalSales"); }

            // 6. Trend Data (7 Days)
            try
            {
                var startDate = DateTime.Today.AddDays(-6);
                var startDateStr = startDate.ToString("yyyy-MM-dd");

                var trendResultsFromDb = await context.sales
                    .AsNoTracking()
                    .Where(s => s.date != null && s.date.CompareTo(startDateStr) >= 0 && !s.is_returned)
                    .GroupBy(s => s.date)
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
            }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error fetching Trend data"); }

            // 8. Sale vs Return
            try
            {
                stats.SaleCount = await context.sales.AsNoTracking().CountAsync(s => !s.is_returned);
                stats.ReturnCount = await context.returns.AsNoTracking().CountAsync();
            }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error fetching Sale/Return counts"); }

            // 9. Low Stock
            try
            {
                stats.LowStockCount = await context.stock_details.AsNoTracking()
                    .CountAsync(s => s.quantity <= s.stock_alert);
            }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error fetching LowStockCount"); }

            // Cache only if we have some data to avoid caching transient failures
            if (stats.TotalCustomers > 0 || stats.TotalProducts > 0)
            {
                _cache.Set(cacheKey, stats, TimeSpan.FromSeconds(30));
            }
            PopulateFromStats(stats);

            _logger.LogInformation("Dashboard load completed in {ms}ms", sw.ElapsedMilliseconds);

            return Page();
        }

        private void PopulateFromStats(DashboardStats stats)
        {
            TotalCustomers = stats.TotalCustomers;
            TotalProducts = stats.TotalProducts;
            TotalSales = stats.TotalSales;
            TrendLabels = stats.TrendLabels;
            TrendData = stats.TrendData;
            SaleCount = stats.SaleCount;
            ReturnCount = stats.ReturnCount;
            LowStockCount = stats.LowStockCount;
        }
    }
}
