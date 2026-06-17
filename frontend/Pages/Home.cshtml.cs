using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Retailio.Data;
using System.IO;

namespace Retailio.Pages
{
    public class HomeModel : PageModel
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public HomeModel(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        // ── Store Info (from admin panel) ────────────────────
        public string  StoreName      { get; set; } = "Retailio";
        public string? StorePhone1    { get; set; }
        public string? StorePhone2    { get; set; }
        public string? StoreAddress   { get; set; }
        public string? StoreCity      { get; set; }
        public string? BusinessNature { get; set; }
        public string? StoreComments  { get; set; }

        // ── Hero stats (from DB) ─────────────────────────────
        public int TotalUsers         { get; set; }
        public int TotalSales         { get; set; }
        public int SatisfactionPct    { get; set; } = 98;

        public async Task OnGetAsync()
        {
            try
            {
                using var ctx = _dbFactory.CreateDbContext();

                var store = await ctx.store_configurations.AsNoTracking().FirstOrDefaultAsync();
                if (store != null)
                {
                    StoreName      = !string.IsNullOrWhiteSpace(store.ShopName) ? store.ShopName : "Retailio";
                    StorePhone1    = store.Phone1;
                    StorePhone2    = store.Phone2;
                    StoreAddress   = store.Address;
                    StoreCity      = store.City;
                    BusinessNature = store.BusinessNature;
                    StoreComments  = store.Comments;
                }

                TotalUsers = await ctx.users.AsNoTracking().CountAsync(u => u.business_id != 0);
                TotalSales = await ctx.SalesHeader.AsNoTracking().CountAsync();
            }
            catch
            {
                // Fallback to defaults if DB unreachable
            }
        }
    }
}
