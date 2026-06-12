using Retailio.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Retailio.Services
{
    public class CurrencyService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private string? _cachedSymbol;

        public CurrencyService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<string> GetSymbolAsync()
        {
            // If already fetched in this request scope, return it
            if (_cachedSymbol != null) return _cachedSymbol;

            try
            {
                using var context = _dbFactory.CreateDbContext();
                var activeCurrency = await context.currencies
                    .FirstOrDefaultAsync(c => c.is_active);

                _cachedSymbol = activeCurrency?.symbol ?? "$";
            }
            catch
            {
                // Fallback to default if DB fails
                _cachedSymbol = "$";
            }
            
            return _cachedSymbol;
        }

        // Synchronous version for simple display in Razor views (uses a default if not yet loaded)
        public string GetSymbol()
        {
            return _cachedSymbol ?? "$";
        }
    }
}
