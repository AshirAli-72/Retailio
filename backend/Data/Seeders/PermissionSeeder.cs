using Retailio.Data;
using Retailio.Models;
using Retailio.Services;

namespace Retailio.backend.Data.Seeders
{
    /// <summary>
    /// Seeds ONLY the permissions table from <see cref="PermissionSlugs.All"/>.
    /// No default roles are created — roles are managed entirely by the Owner
    /// through the Roles &amp; Permissions UI.
    ///
    /// Also cleans up:
    ///   - Obsolete permission slugs no longer in PermissionSlugs.All
    ///   - Any system-seeded roles (business_id = 0) that were previously
    ///     inserted by an old version of this seeder (Manager, Cashier, etc.)
    /// </summary>
    public static class PermissionSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            try
            {
                var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

                // ── 1. Remove any system-seeded roles left over from old seeder ──
                //      These are roles with business_id = 0 that were auto-created.
                //      SuperAdmin (id = 1 or title = "SuperAdmin") is intentionally
                //      excluded — only cleanup previously seeded template roles.
                var systemRolesToRemove = db.roles
                    .Where(r => r.business_id == 0 && r.RoleTitle != "SuperAdmin")
                    .ToList();

                if (systemRolesToRemove.Count > 0)
                {
                    // Remove their roles_has_permissions links first (FK constraint)
                    var roleIds = systemRolesToRemove.Select(r => r.Id).ToList();
                    var links = db.roles_has_permissions
                        .Where(rp => roleIds.Contains(rp.RoleId))
                        .ToList();

                    if (links.Count > 0)
                        db.roles_has_permissions.RemoveRange(links);

                    db.roles.RemoveRange(systemRolesToRemove);
                    db.SaveChanges();

                    Console.WriteLine($"[PermissionSeeder] Removed {systemRolesToRemove.Count} stale " +
                                      $"system role(s): {string.Join(", ", systemRolesToRemove.Select(r => r.RoleTitle))}");
                }

                // ── 2. Remove obsolete permission slugs ───────────────────────────
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

                // ── 3. Insert any missing permission rows ─────────────────────────
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
                    Console.WriteLine("[PermissionSeeder] All permissions are up-to-date. Nothing to seed.");
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
