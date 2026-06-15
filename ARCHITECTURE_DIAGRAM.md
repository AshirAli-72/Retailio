# Retailio Architecture Diagram

## System Overview
Retailio is a multi-tenant Point of Sale (POS) management system built with .NET 10.0, designed for retail businesses to manage sales, inventory, customers, employees, and reporting.

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           RETAILIO ARCHITECTURE                              │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                         CLIENT LAYER                                │   │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              │   │
│  │  │  Razor Pages │  │ Blazor Comp. │  │   Static     │              │   │
│  │  │  (Frontend)  │  │  (Interactive│  │   Files     │              │   │
│  │  │              │  │     UI)      │  │ (Images/CSS) │              │   │
│  │  └──────────────┘  └──────────────┘  └──────────────┘              │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                    │                                        │
│                                    ▼                                        │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                      APPLICATION LAYER                              │   │
│  │  ┌──────────────────────────────────────────────────────────────┐   │   │
│  │  │                    Program.cs (Startup)                     │   │   │
│  │  │  - Service Registration (DI Container)                      │   │   │
│  │  │  - Middleware Pipeline (CORS, Session, Static Files)        │   │   │
│  │  │  - Endpoint Mapping (Controllers, Razor Pages, Blazor Hub)  │   │   │
│  │  └──────────────────────────────────────────────────────────────┘   │   │
│  │                                                                       │   │
│  │  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐     │   │
│  │  │   Controllers    │  │     Services     │  │   Components     │     │   │
│  │  │                  │  │                  │  │                  │     │   │
│  │  │ StatsController  │  │ CurrencyService  │  │ Customer/        │     │   │
│  │  │ (API Endpoints)  │  │ PaymentHelper    │  │ Employee/        │     │   │
│  │  │                  │  │ TenantQuery      │  │ Product/         │     │   │
│  │  └──────────────────┘  └──────────────────┘  │ Reports/         │     │   │
│  │                                              │ Sale/            │     │   │
│  │                                              │ Settings/        │     │   │
│  │                                              └──────────────────┘     │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                    │                                        │
│                                    ▼                                        │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                         DATA LAYER                                  │   │
│  │  ┌──────────────────────────────────────────────────────────────┐   │   │
│  │  │              ApplicationDbContext (EF Core)                │   │   │
│  │  │  - DbContext with 18 DbSets                                │   │   │
│  │  │  - Model configurations & relationships                     │   │   │
│  │  │  - Seeded data (SuperAdmin user & role)                    │   │   │
│  │  └──────────────────────────────────────────────────────────────┘   │   │
│  │                                                                       │   │
│  │  ┌──────────────────────────────────────────────────────────────┐   │   │
│  │  │                      Models (21 entities)                   │   │   │
│  │  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐          │   │   │
│  │  │  │ customers   │ │ products_   │ │ categories  │          │   │   │
│  │  │  │             │ │ services    │ │             │          │   │   │
│  │  │  └─────────────┘ └─────────────┘ └─────────────┘          │   │   │
│  │  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐          │   │   │
│  │  │  │ brands      │ │ users       │ │ roles       │          │   │   │
│  │  │  └─────────────┘ └─────────────┘ └─────────────┘          │   │   │
│  │  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐          │   │   │
│  │  │  │ SaleHeader  │ │ Sale        │ │ ReturnDetail│          │   │   │
│  │  │  └─────────────┘ └─────────────┘ └─────────────┘          │   │   │
│  │  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐          │   │   │
│  │  │  │ Credit      │ │ CreditDetail│ │ Recovery    │          │   │   │
│  │  │  └─────────────┘ └─────────────┘ └─────────────┘          │   │   │
│  │  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐          │   │   │
│  │  │  │ Employee    │ │ StockDetail │ │ stock_      │          │   │   │
│  │  │  │             │ │             │ │ history     │          │   │   │
│  │  │  └─────────────┘ └─────────────┘ └─────────────┘          │   │   │
│  │  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐          │   │   │
│  │  │  │ Currency    │ │ StoreConfig │ │ Business    │          │   │   │
│  │  │  └─────────────┘ └─────────────┘ └─────────────┘          │   │   │
│  │  │  ┌─────────────┐                                            │   │   │
│  │  │  │ Subscription │                                            │   │   │
│  │  │  └─────────────┘                                            │   │   │
│  │  └──────────────────────────────────────────────────────────────┘   │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                    │                                        │
│                                    ▼                                        │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                      DATABASE LAYER                                   │   │
│  │  ┌──────────────────────────────────────────────────────────────┐   │   │
│  │  │                    SQL Server Database                         │   │   │
│  │  │  - Multi-tenant data isolation via user_id foreign keys       │   │   │
│  │  │  - EF Core Migrations for schema management                    │   │   │
│  │  │  - Retry logic on connection failures                         │   │   │
│  │  └──────────────────────────────────────────────────────────────┘   │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Key Architecture Patterns

### 1. Multi-Tenancy
- **Tenant Isolation**: All data queries are scoped to the logged-in user via `TenantQuery` extension methods
- **User-Based Segregation**: Each admin user owns an isolated POS; employees inherit the admin's user_id
- **Session Management**: User ID stored in session for tenant context

### 2. Data Access
- **Repository Pattern**: ApplicationDbContext serves as the repository with EF Core
- **DbContext Factory**: Uses IDbContextFactory for scoped context creation
- **Query Scoping**: TenantQuery extension methods automatically filter data by tenant

### 3. Service Layer
- **CurrencyService**: Manages currency symbols with caching per tenant
- **PaymentHelper**: Handles payment processing logic
- **TenantQuery**: Provides tenant-scoped query extensions for all entities

### 4. Frontend Architecture
- **Razor Pages**: Server-side rendered pages in `/frontend/Pages/`
- **Blazor Components**: Interactive components in `/backend/Components/`
- **Shared Layout**: Common layout in `/frontend/Pages/Shared/_Layout.cshtml`

## Technology Stack

### Core Framework
- **.NET 10.0** (Target Framework)
- **ASP.NET Core** (Web Application)
- **Entity Framework Core 10.0.2** (ORM)
- **SQL Server** (Database)

### UI/Presentation
- **Razor Pages** (Server-side rendering)
- **Blazor Server** (Interactive components)
- **SixLabors.ImageSharp 3.1.12** (Image processing)

### Cross-Cutting Concerns
- **Session Management** (60-minute timeout)
- **CORS** (AllowAnyOrigin for marketing site)
- **Response Compression** (HTTPS enabled)
- **Memory Cache** (Currency caching)

## Directory Structure

```
Retailio/
├── Program.cs                    # Application entry point & startup configuration
├── Retailio.csproj              # Project configuration & dependencies
├── appsettings.json             # Application configuration
├── backend/
│   ├── Controllers/             # API controllers (StatsController)
│   ├── Data/                    # EF Core DbContext & Migrations
│   │   └── ApplicationDbContext.cs
│   ├── Models/                  # 21 entity models
│   ├── Services/                # Business logic services
│   │   ├── CurrencyService.cs
│   │   ├── PaymentHelper.cs
│   │   └── TenantQuery.cs
│   └── Components/              # Blazor components by feature
│       ├── Customer/
│       ├── Employee/
│       ├── Product/
│       ├── Reports/
│       ├── Sale/
│       └── Settings/
├── frontend/
│   └── Pages/                   # Razor Pages by feature
│       ├── Account/
│       ├── Admin/
│       ├── Customer/
│       ├── Employee/
│       ├── Inventory/
│       ├── Product/
│       ├── Recovery/
│       ├── Reports/
│       ├── Sale/
│       ├── Settings/
│       └── Shared/
├── wwwroot/                     # Static files (CSS, JS, images)
├── Models/                      # Additional models (if any)
└── Services/                    # Additional services (if any)
```

## Data Flow

### 1. User Request Flow
```
Client Browser → Razor Page/Blazor Component → Controller/Service → 
ApplicationDbContext → SQL Server → Response → Client
```

### 2. Multi-Tenant Query Flow
```
Session (UserId) → TenantQuery.ForTenant(query) → 
Filtered Query → ApplicationDbContext → SQL Server
```

### 3. Static File Serving
```
/bin/Debug/images → /product-images route
/bin/Debug/Logo → /store-logo route
/bin/Debug/emp_image → /emp-image route
wwwroot/ → Default static files
```

## Key Features by Module

### Sales Management
- SaleHeader (gross total, discount, net payable, paid, due)
- Sale (individual line items with unit/total prices)
- ReturnDetail (product returns)
- Payment status tracking (Paid/Pending)

### Inventory Management
- ProductService (products and services)
- Category & Brand classification
- StockDetail (purchase price, sale price, wholesale price)
- StockHistory (price change tracking)

### Customer Management
- Customer profiles (name, CNIC, contact, address, email)
- Credit limit management
- Credit & CreditDetail tracking
- Recovery system for credit payments

### Employee Management
- Employee profiles with salary
- Role-based access control (RBAC)
- Role & RolePermission system
- User authentication (hashed passwords)

### Reporting
- Sales Reports (SalesReportsHub)
- Inventory Reports (InventoryReportsHub)
- Customer Reports (CustomerReportsHub)
- Credit Reports (CreditReportsHub)
- Employee Reports (EmployeeReportsHub)
- Product Reports (ProductReportsHub)

### Store Configuration
- Store details (name, owner, address, phone)
- Business nature & branch info
- Logo management
- Multi-currency support

## Security Features

### Authentication
- Session-based authentication
- Password hashing (SHA-256)
- SuperAdmin seeded user (username: superadmin)

### Authorization
- Role-based access control
- RolePermission system for granular permissions
- Tenant isolation (users can only access their own data)

### Data Protection
- HttpOnly cookies for session
- SameSite cookie policy (Lax)
- HTTPS redirection in production
- HSTS enabled in production

## Deployment Considerations

### Database
- SQL Server with connection retry logic
- 60-second command timeout
- Automatic migrations via EF Core

### Static Files
- Image directories auto-created on startup
- Separate routes for different image types
- Response compression enabled

### Performance
- Memory caching for currency symbols
- Response compression for all MIME types
- Distributed memory cache for sessions

## API Endpoints

### Stats API
- `GET /api/Stats` - Returns dashboard statistics
  - activeUsers: Count of active customers
  - totalSales: Sum of net payable from sales
  - satisfactionPct: Sales satisfaction percentage (min 85%)
