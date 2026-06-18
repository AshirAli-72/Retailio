namespace Retailio.Services
{
    /// <summary>
    /// Granular permission slugs used throughout the application.
    /// "view_*" slugs are removed for CRUD sections — having any action permission
    /// (create/edit/delete) grants access to that section's index page.
    /// Exceptions: view_report (reports hub access), view_settings, view_dashboard,
    /// view_users, view_roles, view_currency, view_about are kept as standalone gates.
    /// </summary>
    public static class PermissionSlugs
    {
        // ── Dashboard ─────────────────────────────────────────────
        public const string ViewDashboard   = "view_dashboard";

        // ── Customers ─────────────────────────────────────────────
        public const string CreateCustomer  = "create_customer";
        public const string EditCustomer    = "edit_customer";
        public const string DeleteCustomer  = "delete_customer";

        // ── Products ──────────────────────────────────────────────
        public const string CreateProduct   = "create_product";
        public const string EditProduct     = "edit_product";
        public const string DeleteProduct   = "delete_product";

        // ── Sales ─────────────────────────────────────────────────
        public const string CreateSale      = "create_sale";
        public const string EditSale        = "edit_sale";
        public const string DeleteSale      = "delete_sale";

        // ── Inventory ─────────────────────────────────────────────
        public const string EditInventory   = "edit_inventory";

        // ── Employees ─────────────────────────────────────────────
        public const string CreateEmployee  = "create_employee";
        public const string EditEmployee    = "edit_employee";
        public const string DeleteEmployee  = "delete_employee";

        // ── Recovery ──────────────────────────────────────────────
        public const string CreateRecovery  = "create_recovery";
        public const string DeleteRecovery  = "delete_recovery";

        // ── Reports ───────────────────────────────────────────────
        public const string ViewReport      = "view_report";   // see reports hub (no print)
        public const string PrintReport     = "print_report";  // print/export

        // ── Settings (hub) ────────────────────────────────────────
        public const string ViewSettings    = "view_settings";

        // ── Settings > Users ──────────────────────────────────────
        public const string ViewUsers       = "view_users";
        public const string CreateUser      = "create_user";
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
            CreateCustomer,  EditCustomer,   DeleteCustomer,
            CreateProduct,   EditProduct,    DeleteProduct,
            CreateSale,      EditSale,       DeleteSale,
            EditInventory,
            CreateEmployee,  EditEmployee,   DeleteEmployee,
            CreateRecovery,  DeleteRecovery,
            ViewReport,      PrintReport,
            ViewSettings,
            ViewUsers,       CreateUser,     EditUser,        DeleteUser,
            ViewRoles,       ManageRoles,
            ViewCurrency,    ManageCurrency,
            ViewAbout,       EditAbout
        };

        // ── Helper: does user have any permission for a given section? ──
        public static bool HasAnyCustomer(HashSet<string> p)  => p.Contains(CreateCustomer) || p.Contains(EditCustomer)  || p.Contains(DeleteCustomer);
        public static bool HasAnyProduct(HashSet<string> p)   => p.Contains(CreateProduct)  || p.Contains(EditProduct)   || p.Contains(DeleteProduct);
        public static bool HasAnySale(HashSet<string> p)      => p.Contains(CreateSale)     || p.Contains(EditSale)      || p.Contains(DeleteSale);
        public static bool HasAnyEmployee(HashSet<string> p)  => p.Contains(CreateEmployee) || p.Contains(EditEmployee)  || p.Contains(DeleteEmployee);
        public static bool HasAnyRecovery(HashSet<string> p)  => p.Contains(CreateRecovery) || p.Contains(DeleteRecovery);
        public static bool HasAnyInventory(HashSet<string> p) => p.Contains(EditInventory);

        /// <summary>Human-readable display name for a slug.</summary>
        public static string GetDisplayName(string slug) => slug switch
        {
            ViewDashboard   => "View Dashboard",
            CreateCustomer  => "Create Customer",
            EditCustomer    => "Edit Customer",
            DeleteCustomer  => "Delete Customer",
            CreateProduct   => "Create Product",
            EditProduct     => "Edit Product",
            DeleteProduct   => "Delete Product",
            CreateSale      => "Create Sale",
            EditSale        => "Edit Sale",
            DeleteSale      => "Delete Sale",
            EditInventory   => "Edit Inventory",
            CreateEmployee  => "Create Employee",
            EditEmployee    => "Edit Employee",
            DeleteEmployee  => "Delete Employee",
            CreateRecovery  => "Create Recovery",
            DeleteRecovery  => "Delete Recovery",
            ViewReport      => "View Reports",
            PrintReport     => "Print Reports",
            ViewSettings    => "View Settings",
            ViewUsers       => "View Users",
            CreateUser      => "Create User",
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
