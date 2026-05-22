CREATE TABLE [brands] (
    [id] int NOT NULL IDENTITY,
    [brand_title] nvarchar(200) NOT NULL,
    CONSTRAINT [PK_brands] PRIMARY KEY ([id])
);
GO


CREATE TABLE [categories] (
    [id] int NOT NULL IDENTITY,
    [category_title] nvarchar(200) NOT NULL,
    CONSTRAINT [PK_categories] PRIMARY KEY ([id])
);
GO


CREATE TABLE [currencies] (
    [id] int NOT NULL IDENTITY,
    [name] nvarchar(max) NULL,
    [code] nvarchar(max) NULL,
    [symbol] nvarchar(max) NULL,
    [exchange_rate] decimal(18,2) NOT NULL,
    [status] nvarchar(max) NULL,
    [is_active] bit NOT NULL,
    CONSTRAINT [PK_currencies] PRIMARY KEY ([id])
);
GO


CREATE TABLE [customers] (
    [id] int NOT NULL IDENTITY,
    [name] nvarchar(max) NULL,
    [contact] nvarchar(max) NULL,
    [address] nvarchar(max) NULL,
    [email] nvarchar(max) NULL,
    [cnic] nvarchar(max) NULL,
    [credit_limit] nvarchar(max) NULL,
    [status] nvarchar(max) NULL,
    CONSTRAINT [PK_customers] PRIMARY KEY ([id])
);
GO


CREATE TABLE [employee] (
    [id] int NOT NULL IDENTITY,
    [date] nvarchar(max) NOT NULL,
    [full_name] nvarchar(max) NOT NULL,
    [emp_code] nvarchar(max) NULL,
    [cnic] nvarchar(max) NULL,
    [email] nvarchar(max) NULL,
    [mobile_no] nvarchar(max) NOT NULL,
    [address] nvarchar(max) NOT NULL,
    [image_path] nvarchar(max) NULL,
    [salary] decimal(18,2) NOT NULL,
    [status] nvarchar(max) NULL,
    CONSTRAINT [PK_employee] PRIMARY KEY ([id])
);
GO


CREATE TABLE [invoices] (
    [id] int NOT NULL IDENTITY,
    [invoice_no] nvarchar(max) NULL,
    [date] nvarchar(max) NOT NULL,
    [seller_name] nvarchar(max) NULL,
    [seller_contact] nvarchar(max) NULL,
    [seller_address] nvarchar(max) NULL,
    [customer_name] nvarchar(max) NULL,
    [customer_address] nvarchar(max) NULL,
    [customer_contact] nvarchar(max) NULL,
    [prod_name/service] nvarchar(max) NULL,
    [qty/unit_type] nvarchar(max) NULL,
    [price] decimal(18,2) NOT NULL,
    [discount] decimal(18,2) NOT NULL,
    [total_price] decimal(18,2) NOT NULL,
    [payment] nvarchar(max) NULL,
    [status] nvarchar(max) NULL,
    [user_id] int NULL,
    CONSTRAINT [PK_invoices] PRIMARY KEY ([id])
);
GO


CREATE TABLE [products_services] (
    [id] int NOT NULL IDENTITY,
    [prod_name] nvarchar(max) NULL,
    [barcode] nvarchar(max) NULL,
    [manufacture_date] nvarchar(max) NULL,
    [expiry_date] nvarchar(max) NULL,
    [prod_state] nvarchar(max) NULL,
    [unit] nvarchar(max) NULL,
    [item_type] nvarchar(max) NULL,
    [size] int NULL,
    [pic] nvarchar(max) NULL,
    [status] nvarchar(max) NULL,
    [remarks] nvarchar(max) NULL,
    [category_id] int NULL,
    [brand_id] int NULL,
    CONSTRAINT [PK_products_services] PRIMARY KEY ([id])
);
GO


CREATE TABLE [return_details] (
    [id] int NOT NULL IDENTITY,
    [billNo] nvarchar(max) NULL,
    [sale_id] int NOT NULL,
    [date] nvarchar(max) NOT NULL,
    [no_of_items] int NOT NULL,
    [qty] decimal(18,2) NOT NULL,
    [total_qty] decimal(18,2) NOT NULL,
    [amount] decimal(18,2) NOT NULL,
    [method] nvarchar(max) NULL,
    [status] nvarchar(max) NULL,
    CONSTRAINT [PK_return_details] PRIMARY KEY ([id])
);
GO


CREATE TABLE [roles] (
    [id] int NOT NULL IDENTITY,
    [role_title] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_roles] PRIMARY KEY ([id])
);
GO


CREATE TABLE [sale_details] (
    [id] int NOT NULL IDENTITY,
    [billNo] nvarchar(max) NULL,
    [date] nvarchar(max) NOT NULL,
    [no_of_items] int NOT NULL,
    [qty] decimal(18,2) NOT NULL,
    [total_qty] decimal(18,2) NOT NULL,
    [price] decimal(18,2) NOT NULL,
    [discount] decimal(18,2) NOT NULL,
    [expiry_date] nvarchar(max) NULL,
    [total_price] decimal(18,2) NOT NULL,
    [description] nvarchar(max) NULL,
    [payment_method] nvarchar(max) NULL,
    [status] nvarchar(max) NULL,
    [is_returned] bit NOT NULL,
    [user_id] int NULL,
    CONSTRAINT [PK_sale_details] PRIMARY KEY ([id])
);
GO


CREATE TABLE [stock_details] (
    [id] int NOT NULL IDENTITY,
    [item_barcode] nvarchar(max) NULL,
    [quantity] decimal(18,2) NOT NULL,
    [pur_price] decimal(18,2) NOT NULL,
    [sale_price] decimal(18,2) NOT NULL,
    [whole_sale_price] decimal(18,2) NOT NULL,
    [stock_alert] decimal(18,2) NOT NULL,
    [date_of_manafacture] nvarchar(max) NULL,
    [date_of_expiry] nvarchar(max) NULL,
    [total_pur_price] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_stock_details] PRIMARY KEY ([id])
);
GO


CREATE TABLE [stock_history] (
    [id] int NOT NULL IDENTITY,
    [date] nvarchar(max) NULL,
    [new_quantity] decimal(18,2) NOT NULL,
    [old_quantity] decimal(18,2) NOT NULL,
    [new_purchase_price] decimal(18,2) NOT NULL,
    [old_purchase_price] decimal(18,2) NOT NULL,
    [new_sale_price] decimal(18,2) NOT NULL,
    [old_sale_price] decimal(18,2) NOT NULL,
    [remarks] nvarchar(max) NULL,
    [user_id] int NULL,
    [product_id] int NOT NULL,
    CONSTRAINT [PK_stock_history] PRIMARY KEY ([id])
);
GO


CREATE TABLE [store_configurations] (
    [id] int NOT NULL IDENTITY,
    [shop_name] nvarchar(200) NULL,
    [owner_name] nvarchar(200) NULL,
    [city] nvarchar(100) NULL,
    [address] nvarchar(500) NULL,
    [business_nature] nvarchar(200) NULL,
    [branch] nvarchar(100) NULL,
    [shop_no] nvarchar(50) NULL,
    [phone_1] nvarchar(50) NULL,
    [phone_2] nvarchar(50) NULL,
    [logo_path] nvarchar(500) NULL,
    [comments] nvarchar(max) NULL,
    CONSTRAINT [PK_store_configurations] PRIMARY KEY ([id])
);
GO


CREATE TABLE [roles_permissions] (
    [id] int NOT NULL IDENTITY,
    [role_id] int NOT NULL,
    [dashboard] bit NOT NULL,
    [customers] bit NOT NULL,
    [products] bit NOT NULL,
    [sales] bit NOT NULL,
    [invoices] bit NOT NULL,
    [employees] bit NOT NULL,
    [Reports] bit NOT NULL,
    [settings] bit NOT NULL,
    [customer_report] bit NOT NULL,
    [sale_report] bit NOT NULL,
    [product_report] bit NOT NULL,
    [invoice_report] bit NOT NULL,
    [employee_report] bit NOT NULL,
    [returns_report] bit NOT NULL,
    [daily_summary] bit NOT NULL,
    [inventory] bit NOT NULL,
    CONSTRAINT [PK_roles_permissions] PRIMARY KEY ([id]),
    CONSTRAINT [FK_roles_permissions_roles_role_id] FOREIGN KEY ([role_id]) REFERENCES [roles] ([id]) ON DELETE CASCADE
);
GO


CREATE TABLE [users] (
    [id] int NOT NULL IDENTITY,
    [email] nvarchar(max) NULL,
    [username] nvarchar(max) NULL,
    [password] nvarchar(max) NULL,
    [role_id] int NOT NULL,
    [emp_id] int NOT NULL,
    [status] nvarchar(max) NULL,
    CONSTRAINT [PK_users] PRIMARY KEY ([id]),
    CONSTRAINT [FK_users_employee_emp_id] FOREIGN KEY ([emp_id]) REFERENCES [employee] ([id]) ON DELETE CASCADE,
    CONSTRAINT [FK_users_roles_role_id] FOREIGN KEY ([role_id]) REFERENCES [roles] ([id]) ON DELETE CASCADE
);
GO
CREATE TABLE [credits] (
    [id] int NOT NULL IDENTITY,
    [customer_id] nvarchar(max) NULL,
    [grand_total] nvarchar(max) NULL,
    [paid_amount] nvarchar(max) NULL,
    [discount] nvarchar(max) NULL,
    [remaining_amount] nvarchar(max) NULL,
    [date] nvarchar(max) NULL,
    [bill_No] nvarchar(max) NULL,
    CONSTRAINT [PK_customers] PRIMARY KEY ([id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'address', N'cnic', N'date', N'email', N'emp_code', N'full_name', N'image_path', N'mobile_no', N'salary', N'status') AND [object_id] = OBJECT_ID(N'[employee]'))
    SET IDENTITY_INSERT [employee] ON;
INSERT INTO [employee] ([id], [address], [cnic], [date], [email], [emp_code], [full_name], [image_path], [mobile_no], [salary], [status])
VALUES (1, N'Admin Address', N'00000-0000000-0', N'1-1-2024', N'admin@pos.com', N'EMP-001', N'Admin', NULL, N'0000-0000000', 0.0, N'Active');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'address', N'cnic', N'date', N'email', N'emp_code', N'full_name', N'image_path', N'mobile_no', N'salary', N'status') AND [object_id] = OBJECT_ID(N'[employee]'))
    SET IDENTITY_INSERT [employee] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'role_title') AND [object_id] = OBJECT_ID(N'[roles]'))
    SET IDENTITY_INSERT [roles] ON;
INSERT INTO [roles] ([id], [role_title])
VALUES (1, N'Admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'role_title') AND [object_id] = OBJECT_ID(N'[roles]'))
    SET IDENTITY_INSERT [roles] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'customer_report', N'customers', N'daily_summary', N'dashboard', N'employee_report', N'employees', N'inventory', N'invoice_report', N'invoices', N'product_report', N'products', N'Reports', N'returns_report', N'role_id', N'sale_report', N'sales', N'settings') AND [object_id] = OBJECT_ID(N'[roles_permissions]'))
    SET IDENTITY_INSERT [roles_permissions] ON;
INSERT INTO [roles_permissions] ([id], [customer_report], [customers], [daily_summary], [dashboard], [employee_report], [employees], [inventory], [invoice_report], [invoices], [product_report], [products], [Reports], [returns_report], [role_id], [sale_report], [sales], [settings])
VALUES (1, CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), 1, CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit));
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'customer_report', N'customers', N'daily_summary', N'dashboard', N'employee_report', N'employees', N'inventory', N'invoice_report', N'invoices', N'product_report', N'products', N'Reports', N'returns_report', N'role_id', N'sale_report', N'sales', N'settings') AND [object_id] = OBJECT_ID(N'[roles_permissions]'))
    SET IDENTITY_INSERT [roles_permissions] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'email', N'emp_id', N'password', N'role_id', N'status', N'username') AND [object_id] = OBJECT_ID(N'[users]'))
    SET IDENTITY_INSERT [users] ON;
INSERT INTO [users] ([id], [email], [emp_id], [password], [role_id], [status], [username])
VALUES (1, N'admin@pos.com', 1, N'admin123', 1, N'Active', N'admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'email', N'emp_id', N'password', N'role_id', N'status', N'username') AND [object_id] = OBJECT_ID(N'[users]'))
    SET IDENTITY_INSERT [users] OFF;
GO


CREATE INDEX [IX_roles_permissions_role_id] ON [roles_permissions] ([role_id]);
GO


CREATE INDEX [IX_users_emp_id] ON [users] ([emp_id]);
GO


CREATE INDEX [IX_users_role_id] ON [users] ([role_id]);
GO


