using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Retailio.Data;
using Retailio.Models;
using Retailio.Services;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace Retailio.Pages
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
            public int TotalEmployees { get; set; }
            public decimal CreditRemaining { get; set; }
            public decimal TotalCreditAmount { get; set; }
            public decimal TotalRefund { get; set; }
        }

        public class RecentSaleItem
        {
            public string InvNo { get; set; } = "";
            public string? CustomerName { get; set; }
            public string Date { get; set; } = "";
            public decimal Amount { get; set; }
            public string? PaymentMethod { get; set; }
            public string? Status { get; set; }
        }

        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public decimal TotalSales { get; set; }
        public string[] TrendLabels { get; set; } = Array.Empty<string>();
        public int[] TrendData { get; set; } = Array.Empty<int>();
        public int SaleCount { get; set; }
        public int ReturnCount { get; set; }
        public int LowStockCount { get; set; }
        public int TotalEmployees { get; set; }
        public decimal CreditRemaining { get; set; }
        public decimal TotalCreditAmount { get; set; }
        public decimal TotalRefund { get; set; }
        public List<RecentSaleItem> RecentSales { get; set; } = new();
        public string? ErrorMessage { get; set; }

        // Subscription status
        public bool IsSubscriptionExpired { get; set; }
        public int? SubscriptionDaysLeft { get; set; }
        public string? SubscriptionPlan { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToPage("/Account/Login");
            }

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            await _currencyService.GetSymbolAsync();

            var userId = HttpContext.Session.GetInt32("UserId");

            // ── Subscription status check ─────────────────────────
            try
            {
                if (userId.HasValue && userId.Value > 0)
                {
                    using var subCtx = _dbFactory.CreateDbContext();
                    var sub = await subCtx.subscriptions.AsNoTracking()
                        .Where(s => s.business_id == userId.Value)
                        .OrderByDescending(s => s.id)
                        .FirstOrDefaultAsync();
                    if (sub != null)
                    {
                        SubscriptionPlan = sub.plan;
                        if (sub.status == (int?)EntityStatus.Inactive)
                        {
                            IsSubscriptionExpired = true;
                            SubscriptionDaysLeft = 0;
                        }
                        else if (!string.IsNullOrEmpty(sub.expires_at) &&
                                 DateTime.TryParse(sub.expires_at, out var expDate))
                        {
                            var daysLeft = (expDate.Date - DateTime.Today).Days;
                            SubscriptionDaysLeft = Math.Max(0, daysLeft);
                            if (daysLeft < 0)
                            {
                                IsSubscriptionExpired = true;
                                SubscriptionDaysLeft = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error checking subscription"); }


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
            try { stats.TotalCustomers = await context.customers.AsNoTracking().ForTenant(userId).CountAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error fetching TotalCustomers"); }

            // 3. Total Products
            try { stats.TotalProducts = await context.products_services.AsNoTracking().ForTenant(userId).CountAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error fetching TotalProducts"); }

            // 4. Total Sales (net of returns)
            try 
            { 
                var saleTotal = await context.SalesHeader.AsNoTracking().ForTenant(userId)
                    .SumAsync(s => (decimal?)s.net_payable) ?? 0;
                var returnTotal = await context.returns.AsNoTracking().ForTenant(userId)
                    .SumAsync(r => (decimal?)r.total_price) ?? 0;
                stats.TotalSales = saleTotal - returnTotal;
            }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error fetching TotalSales"); }

            // 6. Trend Data (7 Days)
            try
            {
                var startDate = DateTime.Today.AddDays(-6);
                var startDateStr = startDate.ToString("yyyy-MM-dd");

                var trendResultsFromDb = await context.SalesHeader
                    .AsNoTracking()
                    .ForTenant(userId)
                    .Where(s => s.date != null && s.date.CompareTo(startDateStr) >= 0)
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
                stats.SaleCount = await context.SalesHeader.AsNoTracking().ForTenant(userId).CountAsync();
                stats.ReturnCount = await context.returns.AsNoTracking().ForTenant(userId).CountAsync();
            }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error fetching Sale/Return counts"); }

            // 9. Low Stock
            try
            {
                stats.LowStockCount = await context.stock_details.AsNoTracking().ForTenant(userId)
                    .CountAsync(s => s.quantity <= s.stock_alert);
            }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error fetching LowStockCount"); }

            // 10. Total Employees
            try
            {
                stats.TotalEmployees = await context.employees.AsNoTracking().ForTenant(userId).CountAsync();
            }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error fetching TotalEmployees"); }

            // 11. Credit Stats
            try
            {
                stats.CreditRemaining = await context.credits.AsNoTracking().ForTenant(userId)
                    .Where(c => c.status != (int?)PaymentStatus.Paid)
                    .SumAsync(c => (decimal?)c.remaining) ?? 0;
                stats.TotalCreditAmount = await context.credits.AsNoTracking().ForTenant(userId)
                    .SumAsync(c => (decimal?)c.total_credit) ?? 0;
            }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error fetching Credit stats"); }

            // 12. Recovery (Dues Paid)
            try
            {
                stats.TotalRefund = await context.recoveries.AsNoTracking().ForTenant(userId).SumAsync(r => (decimal?)r.paid) ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard: Error fetching TotalRefund from recoveries");
            }

            // 13. Recent Sales
            try
            {
                RecentSales = await context.SalesHeader.AsNoTracking().ForTenant(userId)
                    .OrderByDescending(s => s.id)
                    .Take(10)
                    .Select(s => new RecentSaleItem
                    {
                        InvNo = s.inv_no ?? "",
                        CustomerName = context.customers.ForTenant(userId).Where(c => c.id == s.customer_id).Select(c => c.name).FirstOrDefault(),
                        Date = s.date,
                        Amount = s.net_payable,
                        PaymentMethod = s.payment_method.HasValue ? PaymentHelper.GetPaymentMethodName(s.payment_method.Value) : null,
                        Status = s.status.HasValue ? PaymentHelper.GetStatusName(s.status.Value) : null
                    }).ToListAsync();
            }
            catch (Exception ex) { _logger.LogError(ex, "Dashboard: Error fetching Recent Sales"); }


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
            TotalEmployees = stats.TotalEmployees;
            CreditRemaining = stats.CreditRemaining;
            TotalCreditAmount = stats.TotalCreditAmount;
            TotalRefund = stats.TotalRefund;
        }
    }
}
