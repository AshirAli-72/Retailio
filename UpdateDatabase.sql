-- COMPREHENSIVE Database Migration Script
-- Converts all payment_method and status columns from string to INT
-- Handles BOTH payment/transaction tables AND entity tables!
-- NEW VALUES START AT 1!
-- Handles "Enable"/"Disable" as synonyms for "Active"/"Inactive"!

SET NOCOUNT ON;
PRINT '=== Starting Comprehensive Database Update ===';

-- =============================================
-- Helper: Check if column is string type
-- =============================================
DECLARE @isString BIT;

-- =============================================
-- 1. Process Payment/Transaction Tables
--    PaymentMethod: Cash(1), Card(2), Online/Transfer(3), Credit(4)
--    PaymentStatus: Paid(1), Pending(2), Returned(3)
-- =============================================
PRINT '';
PRINT '=== Processing Payment/Transaction Tables ===';

-- sales table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'sales')
BEGIN
    PRINT 'Processing sales table...';
    -- payment_method
    SET @isString = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('sales') AND name = 'payment_method' AND system_type_id IN (35, 99, 167, 175, 231, 239))
    BEGIN
        SET @isString = 1;
        PRINT '  Updating payment_method...';
        UPDATE sales SET payment_method = CASE 
            WHEN payment_method = 'Cash' THEN '1'
            WHEN payment_method = 'Card' THEN '2'
            WHEN payment_method IN ('Transfer', 'Online') THEN '3'
            WHEN payment_method = 'Credit' THEN '4'
            ELSE NULL
        END;
        PRINT '  Altering payment_method to INT...';
        ALTER TABLE sales ALTER COLUMN payment_method INT NULL;
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('sales') AND name = 'payment_method')
    BEGIN
        PRINT '  payment_method already INT, skipping...';
    END

    -- status
    SET @isString = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('sales') AND name = 'status' AND system_type_id IN (35, 99, 167, 175, 231, 239))
    BEGIN
        SET @isString = 1;
        PRINT '  Updating status...';
        UPDATE sales SET status = CASE 
            WHEN status = 'Paid' THEN '1'
            WHEN status = 'Pending' THEN '2'
            WHEN status = 'Returned' THEN '3'
            ELSE '2'
        END;
        PRINT '  Altering status to INT...';
        ALTER TABLE sales ALTER COLUMN status INT NULL;
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('sales') AND name = 'status')
    BEGIN
        PRINT '  status already INT, skipping...';
    END
END

-- sale_details table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'sale_details')
BEGIN
    PRINT 'Processing sale_details table...';
    SET @isString = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('sale_details') AND name = 'status' AND system_type_id IN (35, 99, 167, 175, 231, 239))
    BEGIN
        SET @isString = 1;
        PRINT '  Updating status...';
        UPDATE sale_details SET status = CASE 
            WHEN status = 'Paid' THEN '1'
            WHEN status = 'Pending' THEN '2'
            WHEN status = 'Returned' THEN '3'
            ELSE '2'
        END;
        PRINT '  Altering status to INT...';
        ALTER TABLE sale_details ALTER COLUMN status INT NULL;
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('sale_details') AND name = 'status')
    BEGIN
        PRINT '  status already INT, skipping...';
    END
END

-- return_details table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'return_details')
BEGIN
    PRINT 'Processing return_details table...';
    -- payment_method
    SET @isString = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('return_details') AND name = 'payment_method' AND system_type_id IN (35, 99, 167, 175, 231, 239))
    BEGIN
        SET @isString = 1;
        PRINT '  Updating payment_method...';
        UPDATE return_details SET payment_method = CASE 
            WHEN payment_method = 'Cash' THEN '1'
            WHEN payment_method = 'Card' THEN '2'
            WHEN payment_method IN ('Transfer', 'Online') THEN '3'
            WHEN payment_method = 'Credit' THEN '4'
            ELSE NULL
        END;
        PRINT '  Altering payment_method to INT...';
        ALTER TABLE return_details ALTER COLUMN payment_method INT NULL;
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('return_details') AND name = 'payment_method')
    BEGIN
        PRINT '  payment_method already INT, skipping...';
    END

    -- status
    SET @isString = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('return_details') AND name = 'status' AND system_type_id IN (35, 99, 167, 175, 231, 239))
    BEGIN
        SET @isString = 1;
        PRINT '  Updating status...';
        UPDATE return_details SET status = CASE 
            WHEN status = 'Paid' THEN '1'
            WHEN status = 'Pending' THEN '2'
            WHEN status = 'Returned' THEN '3'
            ELSE '3'
        END;
        PRINT '  Altering status to INT...';
        ALTER TABLE return_details ALTER COLUMN status INT NULL;
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('return_details') AND name = 'status')
    BEGIN
        PRINT '  status already INT, skipping...';
    END
END

-- credits table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'credits')
BEGIN
    PRINT 'Processing credits table...';
    -- payment_method
    SET @isString = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('credits') AND name = 'payment_method' AND system_type_id IN (35, 99, 167, 175, 231, 239))
    BEGIN
        SET @isString = 1;
        PRINT '  Updating payment_method...';
        UPDATE credits SET payment_method = CASE 
            WHEN payment_method = 'Cash' THEN '1'
            WHEN payment_method = 'Card' THEN '2'
            WHEN payment_method IN ('Transfer', 'Online') THEN '3'
            WHEN payment_method = 'Credit' THEN '4'
            ELSE NULL
        END;
        PRINT '  Altering payment_method to INT...';
        ALTER TABLE credits ALTER COLUMN payment_method INT NULL;
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('credits') AND name = 'payment_method')
    BEGIN
        PRINT '  payment_method already INT, skipping...';
    END

    -- status
    SET @isString = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('credits') AND name = 'status' AND system_type_id IN (35, 99, 167, 175, 231, 239))
    BEGIN
        SET @isString = 1;
        PRINT '  Updating status...';
        UPDATE credits SET status = CASE 
            WHEN status = 'Paid' THEN '1'
            WHEN status = 'Pending' THEN '2'
            WHEN status = 'Returned' THEN '3'
            ELSE '2'
        END;
        PRINT '  Altering status to INT...';
        ALTER TABLE credits ALTER COLUMN status INT NULL;
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('credits') AND name = 'status')
    BEGIN
        PRINT '  status already INT, skipping...';
    END
END

-- credits_details table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'credits_details')
BEGIN
    PRINT 'Processing credits_details table...';
    -- payment_method
    SET @isString = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('credits_details') AND name = 'payment_method' AND system_type_id IN (35, 99, 167, 175, 231, 239))
    BEGIN
        SET @isString = 1;
        PRINT '  Updating payment_method...';
        UPDATE credits_details SET payment_method = CASE 
            WHEN payment_method = 'Cash' THEN '1'
            WHEN payment_method = 'Card' THEN '2'
            WHEN payment_method IN ('Transfer', 'Online') THEN '3'
            WHEN payment_method = 'Credit' THEN '4'
            ELSE NULL
        END;
        PRINT '  Altering payment_method to INT...';
        ALTER TABLE credits_details ALTER COLUMN payment_method INT NULL;
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('credits_details') AND name = 'payment_method')
    BEGIN
        PRINT '  payment_method already INT, skipping...';
    END

    -- status
    SET @isString = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('credits_details') AND name = 'status' AND system_type_id IN (35, 99, 167, 175, 231, 239))
    BEGIN
        SET @isString = 1;
        PRINT '  Updating status...';
        UPDATE credits_details SET status = CASE 
            WHEN status = 'Paid' THEN '1'
            WHEN status = 'Pending' THEN '2'
            WHEN status = 'Returned' THEN '3'
            ELSE '2'
        END;
        PRINT '  Altering status to INT...';
        ALTER TABLE credits_details ALTER COLUMN status INT NULL;
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('credits_details') AND name = 'status')
    BEGIN
        PRINT '  status already INT, skipping...';
    END
END

-- recoveries table (NO payment_method!)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'recoveries')
BEGIN
    PRINT 'Processing recoveries table...';
    SET @isString = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('recoveries') AND name = 'status' AND system_type_id IN (35, 99, 167, 175, 231, 239))
    BEGIN
        SET @isString = 1;
        PRINT '  Updating status...';
        UPDATE recoveries SET status = CASE 
            WHEN status = 'Paid' THEN '1'
            WHEN status = 'Pending' THEN '2'
            WHEN status = 'Returned' THEN '3'
            ELSE '2'
        END;
        PRINT '  Altering status to INT...';
        ALTER TABLE recoveries ALTER COLUMN status INT NULL;
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('recoveries') AND name = 'status')
    BEGIN
        PRINT '  status already INT, skipping...';
    END
END

-- =============================================
-- 2. Process Entity Tables
--    EntityStatus: Active/Enable(1), Inactive/Disable(2)
-- =============================================
PRINT '';
PRINT '=== Processing Entity Tables ===';

-- users table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'users')
BEGIN
    PRINT 'Processing users table...';
    SET @isString = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('users') AND name = 'status' AND system_type_id IN (35, 99, 167, 175, 231, 239))
    BEGIN
        SET @isString = 1;
        PRINT '  Updating status...';
        UPDATE users SET status = CASE 
            WHEN status IN ('Active', 'Enable') THEN '1'
            WHEN status IN ('Inactive', 'Disable') THEN '2'
            ELSE '1'
        END;
        PRINT '  Altering status to INT...';
        ALTER TABLE users ALTER COLUMN status INT NULL;
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('users') AND name = 'status')
    BEGIN
        PRINT '  status already INT, skipping...';
    END
END

-- customers table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'customers')
BEGIN
    PRINT 'Processing customers table...';
    SET @isString = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('customers') AND name = 'status' AND system_type_id IN (35, 99, 167, 175, 231, 239))
    BEGIN
        SET @isString = 1;
        PRINT '  Updating status...';
        UPDATE customers SET status = CASE 
            WHEN status IN ('Active', 'Enable') THEN '1'
            WHEN status IN ('Inactive', 'Disable') THEN '2'
            ELSE '1'
        END;
        PRINT '  Altering status to INT...';
        ALTER TABLE customers ALTER COLUMN status INT NULL;
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('customers') AND name = 'status')
    BEGIN
        PRINT '  status already INT, skipping...';
    END
END

-- employee table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'employee')
BEGIN
    PRINT 'Processing employee table...';
    SET @isString = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('employee') AND name = 'status' AND system_type_id IN (35, 99, 167, 175, 231, 239))
    BEGIN
        SET @isString = 1;
        PRINT '  Updating status...';
        UPDATE employee SET status = CASE 
            WHEN status IN ('Active', 'Enable') THEN '1'
            WHEN status IN ('Inactive', 'Disable') THEN '2'
            ELSE '1'
        END;
        PRINT '  Altering status to INT...';
        ALTER TABLE employee ALTER COLUMN status INT NULL;
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('employee') AND name = 'status')
    BEGIN
        PRINT '  status already INT, skipping...';
    END
END

-- products_services table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'products_services')
BEGIN
    PRINT 'Processing products_services table...';
    SET @isString = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('products_services') AND name = 'status' AND system_type_id IN (35, 99, 167, 175, 231, 239))
    BEGIN
        SET @isString = 1;
        PRINT '  Updating status...';
        UPDATE products_services SET status = CASE 
            WHEN status IN ('Active', 'Enable') THEN '1'
            WHEN status IN ('Inactive', 'Disable') THEN '2'
            ELSE '1'
        END;
        PRINT '  Altering status to INT...';
        ALTER TABLE products_services ALTER COLUMN status INT NULL;
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('products_services') AND name = 'status')
    BEGIN
        PRINT '  status already INT, skipping...';
    END
END

PRINT '';
PRINT '=== Comprehensive Database Update Complete! ===';
