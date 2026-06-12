
using System.Security.Cryptography;
using System.Text;

namespace Retailio.Services
{
    public enum PaymentMethod
    {
        Cash = 1,
        Card = 2,
        Online = 3,
        Credit = 4
    }

    public enum PaymentStatus
    {
        Paid = 1,
        Pending = 2,
        Returned = 3
    }

    public enum EntityStatus
    {
        Active = 1,
        Inactive = 2
    }

    public static class PaymentHelper
    {
        public static string GetPaymentMethodName(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.Cash => "Cash",
                PaymentMethod.Card => "Card",
                PaymentMethod.Online => "Online",
                PaymentMethod.Credit => "Credit",
                _ => "Unknown"
            };
        }

        public static string GetPaymentMethodName(int? method)
        {
            if (!method.HasValue) return "Unknown";
            return GetPaymentMethodName((PaymentMethod)method.Value);
        }

        public static string GetPaymentStatusName(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Paid => "Paid",
                PaymentStatus.Pending => "Pending",
                PaymentStatus.Returned => "Returned",
                _ => "Unknown"
            };
        }

        public static string GetPaymentStatusName(int? status)
        {
            if (!status.HasValue) return "Pending";
            return GetPaymentStatusName((PaymentStatus)status.Value);
        }

        // Alias for compatibility
        public static string GetStatusName(PaymentStatus status) => GetPaymentStatusName(status);
        public static string GetStatusName(int? status) => GetPaymentStatusName(status);

        public static string GetEntityStatusName(EntityStatus status)
        {
            return status switch
            {
                EntityStatus.Active => "Active",
                EntityStatus.Inactive => "Inactive",
                _ => "Unknown"
            };
        }

        public static string GetEntityStatusName(int? status)
        {
            if (!status.HasValue) return "Active";
            return GetEntityStatusName((EntityStatus)status.Value);
        }

        // ── Role helpers ──────────────────────────────────────────────────
        /// <summary>
        /// Returns the role title for a given role_id by looking it up from the
        /// provided role title string. Since roles are now fully dynamic (admins
        /// can create any role), callers should pass user.Role?.RoleTitle directly.
        /// This overload is kept as a fallback that returns "Unknown" when no title
        /// is available.
        /// </summary>
        public static string GetRoleName(int? roleId) => "Unknown";

        public static bool IsSuperAdmin(string? roleTitle) =>
            string.Equals(roleTitle, "SuperAdmin", StringComparison.OrdinalIgnoreCase);

        /// <summary>Returns true for SuperAdmin only (admin panel access).</summary>
        public static bool HasAdminPanelAccess(string? roleTitle) =>
            IsSuperAdmin(roleTitle);

        /// <summary>
        /// Returns true for any user that has a non-empty role — since roles are
        /// dynamic, every assigned role is considered to have POS access.
        /// SuperAdmin is the only role that goes to the admin panel instead.
        /// </summary>
        public static bool HasPosAccess(string? roleTitle) =>
            !string.IsNullOrWhiteSpace(roleTitle);

        // Parse methods
        public static PaymentMethod? ParsePaymentMethod(string? methodName)
        {
            return methodName switch
            {
                "Cash"     => PaymentMethod.Cash,
                "Card"     => PaymentMethod.Card,
                "Transfer" => PaymentMethod.Online,
                "Online"   => PaymentMethod.Online,
                "Credit"   => PaymentMethod.Credit,
                _ => null
            };
        }

        public static PaymentStatus? ParsePaymentStatus(string? statusName)
        {
            return statusName switch
            {
                "Paid"     => PaymentStatus.Paid,
                "Pending"  => PaymentStatus.Pending,
                "Returned" => PaymentStatus.Returned,
                _ => null
            };
        }

        public static EntityStatus? ParseEntityStatus(string? statusName)
        {
            return statusName switch
            {
                "Active"   or "Enable"  => EntityStatus.Active,
                "Inactive" or "Disable" => EntityStatus.Inactive,
                _ => null
            };
        }

        // ── Password hashing ──────────────────────────────────────────────
        /// <summary>Returns the SHA-256 hex digest of the given plain-text password.</summary>
        public static string HashPassword(string plainText)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plainText));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        /// <summary>Returns true when the plain-text matches the stored hash.</summary>
        public static bool VerifyPassword(string plainText, string? storedHash)
            => !string.IsNullOrEmpty(storedHash) &&
               string.Equals(HashPassword(plainText), storedHash, StringComparison.OrdinalIgnoreCase);
    }
}
