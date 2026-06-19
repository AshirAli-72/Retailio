using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Retailio.Data;

namespace Retailio.Services
{
    /// <summary>
    /// Central service for checking granular user permissions.
    /// Owners / Admins always receive full access.
    /// Employees have permissions resolved via user_has_roles → roles_has_permissions → permissions.
    /// </summary>
    public class PermissionService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;

        public PermissionService(
            IDbContextFactory<ApplicationDbContext> dbFactory,
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache cache)
        {
            _dbFactory = dbFactory;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
        }

        private ISession? Session => _httpContextAccessor.HttpContext?.Session;

        /// <summary>Returns true if the current user is an Owner or SuperAdmin (bypasses all permission checks).</summary>
        public async Task<bool> IsOwnerOrAdminAsync()
        {
            var role = Session?.GetString("UserRole") ?? "";
            if (role.Equals("Owner", StringComparison.OrdinalIgnoreCase)
                || PaymentHelper.HasAdminPanelAccess(role)
                || role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Also check database: if user is not in user_has_roles, treat as owner
            var userAccountId = Session?.GetInt32("UserAccountId");
            if (!userAccountId.HasValue)
                return false;

            try
            {
                using var ctx = _dbFactory.CreateDbContext();
                var hasRoles = await ctx.user_has_roles.AnyAsync(uhr => uhr.UserId == userAccountId);
                return !hasRoles;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Loads all permission slugs for the current user (cached for 30 minutes).</summary>
        public async Task<HashSet<string>> GetUserPermissionsAsync()
        {
            if (await IsOwnerOrAdminAsync())
                return new HashSet<string>(PermissionSlugs.All, StringComparer.OrdinalIgnoreCase);

            var userAccountId = Session?.GetInt32("UserAccountId");
            if (!userAccountId.HasValue)
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            string cacheKey = $"Perms_u_{userAccountId}";
            if (_cache.TryGetValue(cacheKey, out HashSet<string>? cached) && cached != null)
                return cached;

            try
            {
                using var ctx = _dbFactory.CreateDbContext();
                var slugs = await ctx.user_has_roles
                    .Where(uhr => uhr.UserId == userAccountId)
                    .Join(ctx.roles_has_permissions,
                        uhr => uhr.RolesHasPermissionId,
                        rp => rp.Id,
                        (uhr, rp) => rp.PermissionId)
                    .Join(ctx.permissions,
                        permId => permId,
                        p => p.Id,
                        (permId, p) => p.Slugs ?? "")
                    .Distinct()
                    .ToListAsync();

                var result = new HashSet<string>(slugs.Where(s => !string.IsNullOrEmpty(s)),
                                                StringComparer.OrdinalIgnoreCase);
                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
                return result;
            }
            catch
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        /// <summary>Checks a single permission slug for the current user.</summary>
        public async Task<bool> HasPermissionAsync(string slug)
        {
            var perms = await GetUserPermissionsAsync();
            return perms.Contains(slug);
        }

        /// <summary>Invalidates the permission cache for a user after role changes.</summary>
        public static void InvalidateCache(IMemoryCache cache, int userAccountId)
            => cache.Remove($"Perms_u_{userAccountId}");
    }
}
