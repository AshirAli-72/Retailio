namespace Retailio.Services
{
    /// <summary>
    /// Granular permission slugs used throughout the application.
    /// </summary>
    public static class PermissionSlugs
    {
        // ── Dashboard ─────────────────────────────────────────────
        public const string ViewDashboard   = "view_dashboard";

        // ── Customers ─────────────────────────────────────────────
        public const string ViewCustomer    = "view_customer";
        public const string CreateCustomer  = "create_customer";
        public const string EditCustomer    = "edit_customer";
        public const string DeleteCustomer  = "delete_customer";

        // ── Products ──────────────────────────────────────────────
        public const string ViewProduct     = "view_product";
        public const string CreateProduct   = "create_product";
        public const string EditProduct     = "edit_product";
        public const string DeleteProduct   = "delete_product";

        // ── Sales ─────────────────────────────────────────────────
        public const string ViewSale        = "view_sale";
        public const string CreateSale      = "create_sale";
        public const string EditSale        = "edit_sale";
        public const string DeleteSale      = "delete_sale";

        // ── Inventory ─────────────────────────────────────────────
        public const string ViewInventory   = "view_inventory";
        public const string EditInventory   = "edit_inventory";

        // ── Employees ─────────────────────────────────────────────
        public const string ViewEmployee    = "view_employee";
        public const string CreateEmployee  = "create_employee";
        public const string EditEmployee    = "edit_employee";
        public const string DeleteEmployee  = "delete_employee";

        // ── Recovery ──────────────────────────────────────────────
        public const string ViewRecovery    = "view_recovery";
        public const string CreateRecovery  = "create_recovery";
        public const string DeleteRecovery  = "delete_recovery";

        // ── Reports ───────────────────────────────────────────────
        public const string ViewReport      = "view_report";   // see reports hub (no print)
        public const string PrintReport     = "print_report";  // print/export

        // ── Settings (hub) ────────────────────────────────────────
        public const string ViewSettings    = "view_settings";

        // ── Settings > Users ──────────────────────────────────────
        public const string ViewUsers       = "view_users";
        public const string EditUser        = "edit_user";
        public const string DeleteUser      = "delete_user";

        // ── Settings > Roles ──────────────────────────────────────
        public const string ViewRoles       = "view_roles";
        public const string ManageRoles     = "manage_roles";

        // ── Settings > Currency ───────────────────────────────────
        public const string ViewCurrency    = "view_currency";
        public const string ManageCurrency  = "manage_currency";

        // ── Settings > About Store ────────────────────────────────
        public const string ViewAbout       = "view_about";
        public const string EditAbout       = "edit_about";

        // ── Master list (used for seeding / "Select All") ─────────
        public static readonly string[] All =
        {
            ViewDashboard,
            ViewCustomer,    CreateCustomer,  EditCustomer,   DeleteCustomer,
            ViewProduct,     CreateProduct,   EditProduct,    DeleteProduct,
            ViewSale,        CreateSale,      EditSale,       DeleteSale,
            ViewInventory,   EditInventory,
            ViewEmployee,    CreateEmployee,  EditEmployee,   DeleteEmployee,
            ViewRecovery,    CreateRecovery,  DeleteRecovery,
            ViewReport,      PrintReport,
            ViewSettings,
            ViewUsers,       EditUser,        DeleteUser,
            ViewRoles,       ManageRoles,
            ViewCurrency,    ManageCurrency,
            ViewAbout,       EditAbout
        };

        // ── Helper: does user have any permission for a given section? ──
        public static bool HasAnyCustomer(HashSet<string> p)  => p.Contains(ViewCustomer) || p.Contains(CreateCustomer) || p.Contains(EditCustomer)  || p.Contains(DeleteCustomer);
        public static bool HasAnyProduct(HashSet<string> p)   => p.Contains(ViewProduct) || p.Contains(CreateProduct)  || p.Contains(EditProduct)   || p.Contains(DeleteProduct);
        public static bool HasAnySale(HashSet<string> p)      => p.Contains(ViewSale) || p.Contains(CreateSale)     || p.Contains(EditSale)      || p.Contains(DeleteSale);
        public static bool HasAnyEmployee(HashSet<string> p)  => p.Contains(ViewEmployee) || p.Contains(CreateEmployee) || p.Contains(EditEmployee)  || p.Contains(DeleteEmployee);
        public static bool HasAnyRecovery(HashSet<string> p)  => p.Contains(ViewRecovery) || p.Contains(CreateRecovery) || p.Contains(DeleteRecovery);
        public static bool HasAnyInventory(HashSet<string> p) => p.Contains(ViewInventory) || p.Contains(EditInventory);

        /// <summary>Human-readable display name for a slug.</summary>
        public static string GetDisplayName(string slug) => slug switch
        {
            ViewDashboard   => "View Dashboard",
            ViewCustomer    => "View Customers",
            CreateCustomer  => "Create Customer",
            EditCustomer    => "Edit Customer",
            DeleteCustomer  => "Delete Customer",
            ViewProduct     => "View Products",
            CreateProduct   => "Create Product",
            EditProduct     => "Edit Product",
            DeleteProduct   => "Delete Product",
            ViewSale        => "View Sales",
            CreateSale      => "Create Sale",
            EditSale        => "Edit Sale",
            DeleteSale      => "Delete Sale",
            ViewInventory   => "View Inventory",
            EditInventory   => "Edit Inventory",
            ViewEmployee    => "View Employees",
            CreateEmployee  => "Create Employee",
            EditEmployee    => "Edit Employee",
            DeleteEmployee  => "Delete Employee",
            ViewRecovery    => "View Recovery",
            CreateRecovery  => "Create Recovery",
            DeleteRecovery  => "Delete Recovery",
            ViewReport      => "View Reports",
            PrintReport     => "Print Reports",
            ViewSettings    => "View Settings",
            ViewUsers       => "View Users",
            EditUser        => "Edit User",
            DeleteUser      => "Delete User",
            ViewRoles       => "View Roles",
            ManageRoles     => "Manage Roles",
            ViewCurrency    => "View Currency",
            ManageCurrency  => "Manage Currency",
            ViewAbout       => "View About Store",
            EditAbout       => "Edit About Store",
            _               => slug
        };
    }
}
