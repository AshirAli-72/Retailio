-- ══════════════════════════════════════════════════════════
-- Schema update script — run once in SSMS
-- Creates missing tables and adds new columns without migrations
-- ══════════════════════════════════════════════════════════

-- ── 1. Create businesses table ───────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'businesses')
BEGIN
    CREATE TABLE [businesses] (
        [id]            int           NOT NULL IDENTITY(1,1),
        [user_id]       int           NOT NULL,
        [business_name] nvarchar(200) NULL,
        [business_type] nvarchar(100) NULL,
        CONSTRAINT [PK_businesses] PRIMARY KEY ([id]),
        CONSTRAINT [FK_businesses_users_user_id]
            FOREIGN KEY ([user_id]) REFERENCES [users]([id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_businesses_user_id] ON [businesses] ([user_id]);
    PRINT 'Created table: businesses';
END
ELSE PRINT 'Table already exists: businesses';

-- ── 2. Create subscriptions table ────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'subscriptions')
BEGIN
    CREATE TABLE [subscriptions] (
        [id]         int          NOT NULL IDENTITY(1,1),
        [user_id]    int          NOT NULL,
        [plan]       nvarchar(50) NULL,
        [started_at] nvarchar(10) NOT NULL,   -- yyyy-MM-dd
        [expires_at] nvarchar(10) NULL,       -- yyyy-MM-dd
        [status]     int          NULL,       -- 1=Active, 2=Inactive
        CONSTRAINT [PK_subscriptions] PRIMARY KEY ([id]),
        CONSTRAINT [FK_subscriptions_users_user_id]
            FOREIGN KEY ([user_id]) REFERENCES [users]([id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_subscriptions_user_id] ON [subscriptions] ([user_id]);
    PRINT 'Created table: subscriptions';
END
ELSE
BEGIN
    -- Fix datetime2 columns if they exist from a previous version
    IF EXISTS (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID('subscriptions')
          AND name = 'started_at'
          AND system_type_id = 42  -- datetime2
    )
    BEGIN
        ALTER TABLE [subscriptions] ALTER COLUMN [started_at] nvarchar(10) NULL;
        ALTER TABLE [subscriptions] ALTER COLUMN [expires_at] nvarchar(10) NULL;
        PRINT 'Converted subscriptions date columns from datetime2 to nvarchar(10)';
    END
    ELSE PRINT 'Table already exists: subscriptions';
END

-- ── 3. Add password, role_id, user_id to employee table ──
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('employee') AND name = 'password'
)
BEGIN
    ALTER TABLE [employee] ADD [password] nvarchar(max) NULL;
    PRINT 'Added column: employee.password';
END
ELSE PRINT 'Column already exists: employee.password';

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('employee') AND name = 'role_id'
)
BEGIN
    ALTER TABLE [employee] ADD [role_id] int NULL;
    PRINT 'Added column: employee.role_id';
END
ELSE PRINT 'Column already exists: employee.role_id';

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('employee') AND name = 'user_id'
)
BEGIN
    ALTER TABLE [employee] ADD [user_id] int NULL;
    PRINT 'Added column: employee.user_id';
END
ELSE PRINT 'Column already exists: employee.user_id';

-- ── 4. Register migrations in EF history ─────────────────
IF NOT EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = '20260613100000_create_business_table'
)
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20260613100000_create_business_table', '10.0.2');

IF NOT EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = '20260613110000_create_subscription_table'
)
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20260613110000_create_subscription_table', '10.0.2');

PRINT 'Schema update complete.';
