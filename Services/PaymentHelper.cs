
namespace E_Invoice_system.Services
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

        // Parse methods
        public static PaymentMethod? ParsePaymentMethod(string? methodName)
        {
            return methodName switch
            {
                "Cash" => PaymentMethod.Cash,
                "Card" => PaymentMethod.Card,
                "Transfer" => PaymentMethod.Online,
                "Online" => PaymentMethod.Online,
                "Credit" => PaymentMethod.Credit,
                _ => null
            };
        }

        public static PaymentStatus? ParsePaymentStatus(string? statusName)
        {
            return statusName switch
            {
                "Paid" => PaymentStatus.Paid,
                "Pending" => PaymentStatus.Pending,
                "Returned" => PaymentStatus.Returned,
                _ => null
            };
        }

        public static EntityStatus? ParseEntityStatus(string? statusName)
        {
            return statusName switch
            {
                "Active" or "Enable" => EntityStatus.Active,
                "Inactive" or "Disable" => EntityStatus.Inactive,
                _ => null
            };
        }
    }
}
