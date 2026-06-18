namespace Retailio.Models
{
    public class PermissionViewModel
    {
        // Dashboard
        public bool CanViewDashboard { get; set; }

        // Customers
        public bool CanViewCustomers { get; set; }
        public bool CanCreateCustomer { get; set; }
        public bool CanEditCustomer { get; set; }
        public bool CanDeleteCustomer { get; set; }

        // Products
        public bool CanViewProducts { get; set; }
        public bool CanCreateProduct { get; set; }
        public bool CanEditProduct { get; set; }
        public bool CanDeleteProduct { get; set; }

        // Sales / Returns / Credits
        public bool CanViewSales { get; set; }
        public bool CanCreateSale { get; set; }
        public bool CanEditSale { get; set; }
        public bool CanDeleteSale { get; set; }

        // Inventory
        public bool CanViewInventory { get; set; }
        public bool CanEditInventory { get; set; }

        // Employees
        public bool CanViewEmployees { get; set; }
        public bool CanCreateEmployee { get; set; }
        public bool CanEditEmployee { get; set; }
        public bool CanDeleteEmployee { get; set; }

        // Recovery
        public bool CanViewRecovery { get; set; }
        public bool CanCreateRecovery { get; set; }
        public bool CanDeleteRecovery { get; set; }

        // Reports
        public bool CanViewReports { get; set; }
        public bool CanPrintReports { get; set; }

        // Settings hub
        public bool CanViewSettings { get; set; }

        // Settings > Users
        public bool CanViewUsers { get; set; }
        public bool CanCreateUser { get; set; }
        public bool CanEditUser { get; set; }
        public bool CanDeleteUser { get; set; }

        // Settings > Roles
        public bool CanViewRoles { get; set; }
        public bool CanManageRoles { get; set; }

        // Settings > Currency
        public bool CanViewCurrency { get; set; }
        public bool CanManageCurrency { get; set; }

        // Settings > About
        public bool CanViewAbout { get; set; }
        public bool CanEditAbout { get; set; }
    }
}

