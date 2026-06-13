-- ══════════════════════════════════════════════════════════
-- Clear users table completely and reset identity to start at 1
-- ══════════════════════════════════════════════════════════

-- Disable FK constraints that reference users
ALTER TABLE [businesses]    NOCHECK CONSTRAINT [FK_businesses_users_user_id];
ALTER TABLE [subscriptions] NOCHECK CONSTRAINT [FK_subscriptions_users_user_id];
ALTER TABLE [users]         NOCHECK CONSTRAINT [FK_users_roles_role_id];

-- Clear dependent tables first
DELETE FROM [subscriptions];
DELETE FROM [businesses];

-- Clear users
DELETE FROM [users];

-- Reset identity so next insert gets id = 1
DBCC CHECKIDENT ('users',         RESEED, 0);
DBCC CHECKIDENT ('businesses',    RESEED, 0);
DBCC CHECKIDENT ('subscriptions', RESEED, 0);

-- Re-enable FK constraints
ALTER TABLE [businesses]    WITH CHECK CHECK CONSTRAINT [FK_businesses_users_user_id];
ALTER TABLE [subscriptions] WITH CHECK CHECK CONSTRAINT [FK_subscriptions_users_user_id];
ALTER TABLE [users]         WITH CHECK CHECK CONSTRAINT [FK_users_roles_role_id];

PRINT 'Users table cleared. Next user will get id = 1.';
