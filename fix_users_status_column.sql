-- Fix the users table status column type from nvarchar(max) to int
-- This fixes the login error: "Unable to cast object of type 'System.String' to type 'System.Int32'"

-- First, convert any existing string status values to int
-- Default to 1 (Active) if conversion fails
UPDATE [users]
SET [status] = CASE 
    WHEN TRY_CAST([status] AS int) IS NOT NULL THEN CAST([status] AS int)
    ELSE 1  -- Default to Active status
END
WHERE ISNUMERIC([status]) = 0 OR [status] IS NULL;

-- Alter the column type from nvarchar(max) to int
ALTER TABLE [users]
ALTER COLUMN [status] int NULL;
