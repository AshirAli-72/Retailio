-- ══════════════════════════════════════════════════════════
-- Clear all application tables (keep seeded roles & superadmin)
-- Run this script directly in SSMS or via sqlcmd
-- ══════════════════════════════════════════════════════════

-- Disable all FK constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

-- ── New tables (subscriptions / businesses) ───────────────
DELETE FROM [subscriptions];
DELETE FROM [businesses];

-- ── Sales / Returns / Credits ─────────────────────────────
DELETE FROM [recoveries];
DELETE FROM [credits_details];
DELETE FROM [credits];
DELETE FROM [return_details];
DELETE FROM [sale_details];
DELETE FROM [sales];

-- ── Inventory & Products ──────────────────────────────────
DELETE FROM [stock_history];
DELETE FROM [stock_details];
DELETE FROM [products_services];
DELETE FROM [brands];
DELETE FROM [categories];

-- ── Customers & Employees ─────────────────────────────────
DELETE FROM [customers];
DELETE FROM [employee];

-- ── Users (keep only superadmin id=1) ─────────────────────
DELETE FROM [users] WHERE [id] != 1;

-- ── Settings ──────────────────────────────────────────────
DELETE FROM [currencies];
DELETE FROM [store_configurations];

-- ── Reseed identity columns ───────────────────────────────
DBCC CHECKIDENT ('subscriptions',       RESEED, 0);
DBCC CHECKIDENT ('businesses',          RESEED, 0);
DBCC CHECKIDENT ('recoveries',          RESEED, 0);
DBCC CHECKIDENT ('credits_details',     RESEED, 0);
DBCC CHECKIDENT ('credits',             RESEED, 0);
DBCC CHECKIDENT ('return_details',      RESEED, 0);
DBCC CHECKIDENT ('sale_details',        RESEED, 0);
DBCC CHECKIDENT ('sales',               RESEED, 0);
DBCC CHECKIDENT ('stock_history',       RESEED, 0);
DBCC CHECKIDENT ('stock_details',       RESEED, 0);
DBCC CHECKIDENT ('products_services',   RESEED, 0);
DBCC CHECKIDENT ('brands',              RESEED, 0);
DBCC CHECKIDENT ('categories',          RESEED, 0);
DBCC CHECKIDENT ('customers',           RESEED, 0);
DBCC CHECKIDENT ('employee',            RESEED, 0);
DBCC CHECKIDENT ('users',               RESEED, 1);  -- next will be id=2
DBCC CHECKIDENT ('currencies',          RESEED, 0);
DBCC CHECKIDENT ('store_configurations',RESEED, 0);

-- Re-enable FK constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';

PRINT 'All tables cleared. Superadmin (id=1) retained.';
