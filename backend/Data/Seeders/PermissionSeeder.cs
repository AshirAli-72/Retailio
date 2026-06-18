using Retailio.Data;
using Retailio.Models;
using Retailio.Services;

namespace Retailio.backend.Data.Seeders
{
    /// <summary>
    /// Seeds the permissions table from <see cref="PermissionSlugs.All"/> (single source of truth),
    /// removes any obsolete slugs, and seeds two default system roles (Manager, Cashier)
    /// with sensible default permission assignments.
    ///
    /// Default roles use business_id = 0 (system-level) so they appear in every tenant.
    /// Roles are only inserted once — existing rows are never modified by this seeder.
    /// </summary>
    public static class PermissionSeeder
    {
        // ── Default role definitions ──────────────────────────────────────────────────
        private static readonly (string Title, string Description, string[] Slugs)[] DefaultRoles =
        {
            (
                Title: "Manager",
                Description: "Full access to daily operations. Cannot manage roles, delete users, or change system settings.",
                Slugs: new[]
                {
                    // Dashboard
                    PermissionSlugs.ViewDashboard,

                    // Customers — full CRUD
                    PermissionSlugs.CreateCustomer,
                    PermissionSlugs.EditCustomer,
                    PermissionSlugs.DeleteCustomer,

                    // Products — full CRUD
                    PermissionSlugs.CreateProduct,
                    PermissionSlugs.EditProduct,
                    PermissionSlugs.DeleteProduct,

                    // Sales — full CRUD
                    PermissionSlugs.CreateSale,
                    PermissionSlugs.EditSale,
                    PermissionSlugs.DeleteSale,

                    // Inventory
                    PermissionSlugs.EditInventory,

                    // Employees — full CRUD
                    PermissionSlugs.CreateEmployee,
                    PermissionSlugs.EditEmployee,
                    PermissionSlugs.DeleteEmployee,

                    // Recovery
                    PermissionSlugs.CreateRecovery,
                    PermissionSlugs.DeleteRecovery,

                    // Reports — view + print
                    PermissionSlugs.ViewReport,
                    PermissionSlugs.PrintReport,

                    // Settings hub access
                    PermissionSlugs.ViewSettings,

                    // Users — view + create + edit (no delete)
                    PermissionSlugs.ViewUsers,
                    PermissionSlugs.CreateUser,
                    PermissionSlugs.EditUser,

                    // Roles — view only (no manage)
                    PermissionSlugs.ViewRoles,

                    // Currency — view only
                    PermissionSlugs.ViewCurrency,

                    // About Store — view + edit
                    PermissionSlugs.ViewAbout,
                    PermissionSlugs.EditAbout,
                }
            ),
            (
                Title: "Cashier",
                Description: "POS operator. Can process sales, manage customers, and view reports.",
                Slugs: new[]
                {
                    // Dashboard
                    PermissionSlugs.ViewDashboard,

                    // Customers — create + edit (no delete)
                    PermissionSlugs.CreateCustomer,
                    PermissionSlugs.EditCustomer,

                    // Sales — create + edit (no delete)
                    PermissionSlugs.CreateSale,
                    PermissionSlugs.EditSale,

                    // Reports — view only (no print)
                    PermissionSlugs.ViewReport,
                }
            ),
        };

        // ─────────────────────────────────────────────────────────────────────────────

        public static void Seed(IServiceProvider serviceProvider)
        {
            try
            {
                var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

                // ── Step 1: Remove obsolete permission slugs ──────────────────────
                var validSlugs = new HashSet<string>(PermissionSlugs.All, StringComparer.OrdinalIgnoreCase);

                var toDelete = db.permissions
                    .AsEnumerable()
                    .Where(p => !string.IsNullOrEmpty(p.Slugs) && !validSlugs.Contains(p.Slugs!))
                    .ToList();

                if (toDelete.Count > 0)
                {
                    db.permissions.RemoveRange(toDelete);
                    db.SaveChanges();
                    Console.WriteLine($"[PermissionSeeder] Removed {toDelete.Count} obsolete permission(s): " +
                                      string.Join(", ", toDelete.Select(p => p.Slugs)));
                }

                // ── Step 2: Insert missing permission rows ────────────────────────
                var existingSlugs = db.permissions
                    .Select(p => p.Slugs)
                    .AsEnumerable()
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(s => s!)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var toInsert = PermissionSlugs.All
                    .Where(slug => !existingSlugs.Contains(slug))
                    .Select(slug => new Permission
                    {
                        Slugs = slug,
                        Names = PermissionSlugs.GetDisplayName(slug)
                    })
                    .ToList();

                if (toInsert.Count > 0)
                {
                    db.permissions.AddRange(toInsert);
                    db.SaveChanges();
                    Console.WriteLine($"[PermissionSeeder] Inserted {toInsert.Count} new permission(s): " +
                                      string.Join(", ", toInsert.Select(p => p.Slugs)));
                }
                else
                {
                    Console.WriteLine("[PermissionSeeder] Permissions are up-to-date.");
                }

                // ── Step 3: Load fresh permission map (slug → id) ─────────────────
                var permMap = db.permissions
                    .Where(p => p.Slugs != null)
                    .AsEnumerable()
                    .ToDictionary(p => p.Slugs!, p => p.Id, StringComparer.OrdinalIgnoreCase);

                // ── Step 4: Seed default roles with their permission assignments ───
                foreach (var (title, description, slugs) in DefaultRoles)
                {
                    // Only insert the role if it doesn't exist yet
                    var existingRole = db.roles
                        .FirstOrDefault(r => r.RoleTitle == title && r.business_id == 0);

                    if (existingRole != null)
                    {
                        Console.WriteLine($"[PermissionSeeder] Role '{title}' already exists — skipping.");
                        continue;
                    }

                    // Create the role
                    var role = new Role
                    {
                        RoleTitle   = title,
                        Description = description,
                        business_id = 0  // system-level, available to all tenants
                    };
                    db.roles.Add(role);
                    db.SaveChanges();

                    // Link each slug → roles_has_permissions row
                    var links = slugs
                        .Where(slug => permMap.ContainsKey(slug))
                        .Select(slug => new RolesHasPermission
                        {
                            RoleId       = role.Id,
                            PermissionId = permMap[slug],
                            business_id  = null  // system-level link
                        })
                        .ToList();

                    if (links.Count > 0)
                    {
                        db.roles_has_permissions.AddRange(links);
                        db.SaveChanges();
                    }

                    Console.WriteLine($"[PermissionSeeder] Created default role '{title}' " +
                                      $"with {links.Count} permission(s).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PermissionSeeder] Seeding failed: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"[PermissionSeeder] Inner: {ex.InnerException.Message}");
            }
        }
    }
}
