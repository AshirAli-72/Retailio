-- Disable constraints temporarily
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

-- Delete data from tables in correct order (child tables first)
DELETE FROM recoveries;
DELETE FROM credits_details;
DELETE FROM sale_details;
DELETE FROM return_details;
DELETE FROM sales; -- This is the SalesHeader table
DELETE FROM credits;

-- Reset identity columns
DBCC CHECKIDENT ('recoveries', RESEED, 0);
DBCC CHECKIDENT ('credits_details', RESEED, 0);
DBCC CHECKIDENT ('sale_details', RESEED, 0);
DBCC CHECKIDENT ('return_details', RESEED, 0);
DBCC CHECKIDENT ('sales', RESEED, 0);
DBCC CHECKIDENT ('credits', RESEED, 0);

-- Re-enable constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
