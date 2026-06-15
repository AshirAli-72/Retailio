using Microsoft.AspNetCore.Http;
using Retailio.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Retailio.Services
{
    public class CurrencyService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string? _cachedSymbol;
        private int? _cachedUserId;

        public CurrencyService(IDbContextFactory<ApplicationDbContext> dbFactory, IHttpContextAccessor httpContextAccessor)
        {
            _dbFactory = dbFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetSymbolAsync(int? userId = null)
        {
            userId ??= _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");

            if (_cachedSymbol != null && _cachedUserId == userId) return _cachedSymbol;

            try
            {
                using var context = _dbFactory.CreateDbContext();
                var activeCurrency = await context.currencies
                    .ForTenant(userId)
                    .FirstOrDefaultAsync(c => c.is_active);

                _cachedSymbol = activeCurrency?.symbol ?? "$";
                _cachedUserId = userId;
            }
            catch
            {
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
