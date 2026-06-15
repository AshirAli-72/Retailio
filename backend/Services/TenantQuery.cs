using Retailio.Models;

namespace Retailio.Services
{
    /// <summary>
    /// Scopes POS queries to the logged-in admin tenant (business_id).
    /// Each registered admin owns an isolated POS; employees inherit the admin's business_id.
    /// </summary>
    public static class TenantQuery
    {
        public static bool HasTenant(int? userId) => userId.HasValue && userId.Value > 0;

        public static IQueryable<customers> ForTenant(this IQueryable<customers> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;

        public static IQueryable<ProductService> ForTenant(this IQueryable<ProductService> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;

        public static IQueryable<SaleHeader> ForTenant(this IQueryable<SaleHeader> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;

        public static IQueryable<Sale> ForTenant(this IQueryable<Sale> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;

        public static IQueryable<ReturnDetail> ForTenant(this IQueryable<ReturnDetail> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;

        public static IQueryable<Credit> ForTenant(this IQueryable<Credit> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;

        public static IQueryable<CreditDetail> ForTenant(this IQueryable<CreditDetail> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;

        public static IQueryable<Employee> ForTenant(this IQueryable<Employee> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;

        public static IQueryable<StoreConfiguration> ForTenant(this IQueryable<StoreConfiguration> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;

        public static IQueryable<Category> ForTenant(this IQueryable<Category> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;

        public static IQueryable<Brand> ForTenant(this IQueryable<Brand> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;

        public static IQueryable<StockDetail> ForTenant(this IQueryable<StockDetail> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;

        public static IQueryable<stock_history> ForTenant(this IQueryable<stock_history> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;

        public static IQueryable<Recovery> ForTenant(this IQueryable<Recovery> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;

        public static IQueryable<Currency> ForTenant(this IQueryable<Currency> query, int? userId) =>
            HasTenant(userId) ? query.Where(x => x.business_id == userId) : query;
    }
}
