using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Retailio.Data;
using Retailio.Models;
using Retailio.Services;

namespace Retailio.Pages.Admin
{
    public class AdminPanelModel : PageModel
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly CurrencyService _currency;

        public AdminPanelModel(IDbContextFactory<ApplicationDbContext> dbFactory, CurrencyService currency)
        {
            _dbFactory = dbFactory;
            _currency  = currency;
        }

        // ── Session-derived display props ─────────────────
        public string AdminUser  { get; set; } = "Admin";
        public string AdminEmail { get; set; } = "";
        public string CurrentPath { get; set; } = "";

        // ── Stats ──────────────────────────────────────────
        public int    TotalUsers      { get; set; }
        public int    TotalCustomers  { get; set; }
        public int    TotalProducts   { get; set; }
        public int    TotalEmployees  { get; set; }
        public decimal TotalSales     { get; set; }
        public decimal TotalRevenue   { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal TotalCredits   { get; set; }
        public decimal TotalRecovered { get; set; }
        public decimal TotalDue       { get; set; }
        public decimal TotalPaid      { get; set; }
        public int    SaleCount       { get; set; }
        public int    ReturnCount     { get; set; }
        public int    LowStockCount   { get; set; }
        public int    ActiveCustomers { get; set; }

        // ── Charts ─────────────────────────────────────────
        public string[] TrendLabels  { get; set; } = Array.Empty<string>();
        public int[]    TrendData    { get; set; } = Array.Empty<int>();

        // ── Lists ──────────────────────────────────────────
        public List<RecentTxRow>  RecentTransactions { get; set; } = new();
        public List<UserRow>      AllUsers           { get; set; } = new();

        public class RecentTxRow
        {
            public string InvNo         { get; set; } = "";
            public string? CustomerName { get; set; }
            public string  Date         { get; set; } = "";
            public decimal Amount       { get; set; }
            public string? Method       { get; set; }
            public string? Status       { get; set; }
        }

        public class UserRow
        {
            public string? Username { get; set; }
            public string? Email    { get; set; }
            public string? Role     { get; set; }
            public string? Status   { get; set; }
        }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Guard: must be logged in AND be SuperAdmin
            var role = HttpContext.Session.GetString("UserRole") ?? "";
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToPage("/Account/Login");

            if (!PaymentHelper.HasAdminPanelAccess(role))
                return RedirectToPage("/Index");

            AdminUser  = HttpContext.Session.GetString("UserName") ?? "Admin";
            AdminEmail = HttpContext.Session.GetString("UserEmail") ?? AdminUser;
            CurrentPath = HttpContext.Request.Path.Value ?? "";

            await _currency.GetSymbolAsync();

            try
            {
                using var ctx = _dbFactory.CreateDbContext();

                // ── Counts ─────────────────────────────────────
                // Exclude SuperAdmin (role_id=1) — count only real app users
                TotalUsers     = await ctx.users.AsNoTracking().CountAsync(u => u.role_id != 1);
                TotalCustomers = await ctx.customers.AsNoTracking().CountAsync();
                ActiveCustomers= await ctx.customers.AsNoTracking().CountAsync(c => c.status == (int?)EntityStatus.Active);
                TotalProducts  = await ctx.products_services.AsNoTracking().CountAsync();
                TotalEmployees = await ctx.employees.AsNoTracking().CountAsync();
                SaleCount      = await ctx.SalesHeader.AsNoTracking().CountAsync();
                ReturnCount    = await ctx.returns.AsNoTracking().CountAsync();
                LowStockCount  = await ctx.stock_details.AsNoTracking().CountAsync(s => s.quantity <= s.stock_alert);

                // ── Financials ─────────────────────────────────
                var grossSales  = await ctx.SalesHeader.AsNoTracking().SumAsync(s => (decimal?)s.net_payable) ?? 0;
                var returnTotal = await ctx.returns.AsNoTracking().SumAsync(r => (decimal?)r.total_price) ?? 0;
                TotalSales      = grossSales - returnTotal;
                TotalRevenue    = await ctx.SalesHeader.AsNoTracking().SumAsync(s => (decimal?)s.paid) ?? 0;

                var monthStart  = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("yyyy-MM-dd");
                var monthEnd    = DateTime.Today.ToString("yyyy-MM-dd");
                MonthlyRevenue  = await ctx.SalesHeader.AsNoTracking()
                    .Where(s => s.date != null && s.date.CompareTo(monthStart) >= 0 && s.date.CompareTo(monthEnd) <= 0)
                    .SumAsync(s => (decimal?)s.net_payable) ?? 0;
                TotalCredits    = await ctx.credits.AsNoTracking().SumAsync(c => (decimal?)c.total_credit) ?? 0;
                TotalRecovered  = await ctx.recoveries.AsNoTracking().SumAsync(r => (decimal?)r.paid) ?? 0;
                TotalDue        = await ctx.SalesHeader.AsNoTracking().SumAsync(s => (decimal?)s.due) ?? 0;
                TotalPaid       = await ctx.SalesHeader.AsNoTracking().SumAsync(s => (decimal?)s.paid) ?? 0;

                // ── 7-day trend ────────────────────────────────
                var startStr = DateTime.Today.AddDays(-6).ToString("yyyy-MM-dd");
                var rawTrend = await ctx.SalesHeader.AsNoTracking()
                    .Where(s => s.date != null && s.date.CompareTo(startStr) >= 0)
                    .GroupBy(s => s.date)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .ToListAsync();

                var labels = new List<string>();
                var data   = new List<int>();
                for (int i = 6; i >= 0; i--)
                {
                    var d = DateTime.Today.AddDays(-i);
                    labels.Add(d.ToString("MMM dd"));
                    var match = rawTrend.FirstOrDefault(r => r.Date == d.ToString("yyyy-MM-dd"));
                    data.Add(match?.Count ?? 0);
                }
                TrendLabels = labels.ToArray();
                TrendData   = data.ToArray();

                // ── Recent Transactions (10) ───────────────────
                RecentTransactions = await ctx.SalesHeader.AsNoTracking()
                    .OrderByDescending(s => s.id)
                    .Take(10)
                    .Select(s => new RecentTxRow
                    {
                        InvNo        = s.inv_no ?? "",
                        CustomerName = ctx.customers.Where(c => c.id == s.customer_id).Select(c => c.name).FirstOrDefault(),
                        Date         = s.date,
                        Amount       = s.net_payable,
                        Method       = s.payment_method.HasValue ? PaymentHelper.GetPaymentMethodName(s.payment_method.Value) : "Cash",
                        Status       = s.status.HasValue ? PaymentHelper.GetStatusName(s.status.Value) : "Paid"
                    })
                    .ToListAsync();

                // ── All Users ──────────────────────────────────
                AllUsers = await ctx.users.AsNoTracking()
                    .Select(u => new UserRow
                    {
                        Username = u.username,
                        Email    = u.email,
                        Role     = u.role_id.ToString(),
                        Status   = u.status == (int?)EntityStatus.Active ? "Active" : "Inactive"
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ADMIN PANEL ERROR] {ex}");
                ErrorMessage = ex.Message;
            }

            return Page();
        }
    }
}
