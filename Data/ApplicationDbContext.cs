using Microsoft.EntityFrameworkCore;
using E_Invoice_system.Models;

namespace E_Invoice_system.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<customers> customers { get; set; }
        public DbSet<ProductService> products_services { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Brand> brands { get; set; }
        public DbSet<users> users { get; set; }
        public DbSet<SaleHeader> SalesHeader { get; set; }
        public DbSet<Sale> sales { get; set; }
        public DbSet<ReturnDetail> returns { get; set; }
        public DbSet<Currency> currencies { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<RolePermission> roles_permissions { get; set; }
        public DbSet<StoreConfiguration> store_configurations { get; set; }
        public DbSet<StockDetail> stock_details { get; set; }
        public DbSet<stock_history> stock_history { get; set; }
        public DbSet<Employee> employees { get; set; }
        public DbSet<Credit> credits { get; set; }
        public DbSet<CreditDetail> credits_details { get; set; }
        public DbSet<Recovery> recoveries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customers Configuration
            modelBuilder.Entity<customers>(entity =>
            {
                entity.HasKey(e => e.id);

                entity.Property(e => e.id)
                      .ValueGeneratedOnAdd()
                      .UseIdentityColumn();

                entity.Property(e => e.name).HasColumnType("nvarchar(max)");
                entity.Property(e => e.cnic).HasColumnType("nvarchar(max)");
                entity.Property(e => e.contact).HasColumnType("nvarchar(max)");
                entity.Property(e => e.address).HasColumnType("nvarchar(max)");
                entity.Property(e => e.email).HasColumnType("nvarchar(max)");
                entity.Property(e => e.credit_limit)
                      .HasColumnType("decimal(18,2)");

                entity.ToTable("customers");
            });

            // Other configurations
            modelBuilder.Entity<SaleHeader>(entity =>
            {
                entity.ToTable("sales");
                entity.Property(s => s.gross_total).HasColumnType("decimal(18,2)");
                entity.Property(s => s.discount).HasColumnType("decimal(18,2)");
                entity.Property(s => s.net_payable).HasColumnType("decimal(18,2)");
                entity.Property(s => s.paid).HasColumnType("decimal(18,2)");
                entity.Property(s => s.due).HasColumnType("decimal(18,2)");
                entity.HasMany(s => s.SaleDetails)
                    .WithOne(d => d.SaleHeader)
                    .HasForeignKey(d => d.sale_id)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.ToTable("sale_details");
                entity.Property(s => s.unit_price).HasColumnType("decimal(18,2)");
                entity.Property(s => s.total_price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<ReturnDetail>(entity =>
            {
                entity.ToTable("return_details");
                entity.Property(r => r.unit_price).HasColumnType("decimal(18,2)");
                entity.Property(r => r.total_price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Currency>()
                .Property(c => c.exchange_rate).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<StockDetail>(entity =>
            {
                entity.Property(e => e.pur_price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.sale_price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.whole_sale_price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.total_pur_price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<stock_history>(entity =>
            {
                entity.Property(e => e.new_purchase_price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.old_purchase_price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.new_sale_price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.old_sale_price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Credit>(entity =>
            {
                entity.Property(e => e.total_credit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.paid).HasColumnType("decimal(18,2)");
                entity.Property(e => e.remaining).HasColumnType("decimal(18,2)");
                // Relationship between Credit and Customer
                entity.HasOne<customers>()
                      .WithMany()
                      .HasForeignKey(e => e.customer_id);
                
                // Relationship between Credit and CreditDetail
                entity.HasMany<CreditDetail>()
                      .WithOne()
                      .HasForeignKey(d => d.credit_id);
            });

            modelBuilder.Entity<CreditDetail>(entity =>
            {
                entity.Property(e => e.amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.paid).HasColumnType("decimal(18,2)");
                entity.Property(e => e.due).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Recovery>(entity =>
            {
                entity.Property(e => e.total_credit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.due).HasColumnType("decimal(18,2)");
                entity.Property(e => e.paid).HasColumnType("decimal(18,2)");
                entity.Property(e => e.remaining).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.salary).HasColumnType("decimal(18,2)");
            });

            // Seed Data
            modelBuilder.Entity<Role>().HasData(new Role { Id = 1, RoleTitle = "Admin" });

            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission
                {
                    Id = 1,
                    RoleId = 1,
                    Dashboard = true,
                    Customers = true,
                    Products = true,
                    Sales = true,
                    Employees = true,
                    Reports = true,
                    Settings = true,
                    Inventory = true
                });

            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    id = 1,
                    date = "1-1-2024",
                    full_name = "Admin",
                    emp_code = "EMP-001",
                    cnic = "00000-0000000-0",
                    email = "admin@pos.com",
                    mobile_no = "0000-0000000",
                    address = "Admin Address",
                    salary = 0,
                    status = (int?)E_Invoice_system.Services.EntityStatus.Active
                });

            modelBuilder.Entity<users>().HasData(
                new users
                {
                    id = 1,
                    username = "admin",
                    password = "admin123",
                    email = "admin@pos.com",
                    role_id = 1,
                    emp_id = 1,
                    status = (int?)E_Invoice_system.Services.EntityStatus.Active
                });
        }
    }
}