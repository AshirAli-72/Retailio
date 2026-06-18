-- drop_user_id_all_except_business.sql
-- Drops ONLY user_id columns from all tables EXCEPT the `business` table.
-- IMPORTANT:
-- - DOES NOT drop user_id from `user_has_roles` (per your instruction).
-- - Drops dependent foreign keys first.
-- - Runs in a best-effort way; if a constraint/column doesn’t exist, it skips.

BEGIN TRY
    BEGIN TRAN;

    DECLARE @tbl sysname;

    DECLARE user_id_cursor CURSOR FOR
    SELECT s.name + '.' + t.name
    FROM sys.columns c
    JOIN sys.tables t ON t.object_id = c.object_id
    JOIN sys.schemas s ON s.schema_id = t.schema_id
    WHERE c.name = 'user_id'
      AND t.name <> 'business'
      -- keep user_id column inside user_has_roles
      AND t.name <> 'user_has_roles';

    OPEN user_id_cursor;
    FETCH NEXT FROM user_id_cursor INTO @tbl;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Drop FKs that reference this table's user_id column
        DECLARE @sql nvarchar(max) = '';

        SELECT @sql = STRING_AGG(
            'IF OBJECT_ID(''' + QUOTENAME(fk.name) + ''',''F'') IS NOT NULL ' +
            'ALTER TABLE ' + QUOTENAME(s_parent.name) + '.' + QUOTENAME(t_parent.name) + ' DROP CONSTRAINT ' + QUOTENAME(fk.name) + ';'
        , CHAR(10))
        FROM sys.foreign_key_columns fkc
        JOIN sys.foreign_keys fk ON fk.object_id = fkc.constraint_object_id
        JOIN sys.tables t_parent ON t_parent.object_id = fk.parent_object_id
        JOIN sys.schemas s_parent ON s_parent.schema_id = t_parent.schema_id
        WHERE OBJECT_SCHEMA_NAME(fkc.referenced_object_id) + '.' + OBJECT_NAME(fkc.referenced_object_id) = @tbl
          AND COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) = 'user_id';

        IF @sql IS NOT NULL AND LEN(@sql) > 0
            EXEC sp_executesql @sql;

        -- Drop the user_id column
        SET @sql = 'IF COL_LENGTH(''' + @tbl + ''',''user_id'') IS NOT NULL
                    BEGIN
                        ALTER TABLE ' + @tbl + ' DROP COLUMN [user_id];
                    END';
        EXEC sp_executesql @sql;

        FETCH NEXT FROM user_id_cursor INTO @tbl;
    END

    CLOSE user_id_cursor;
    DEALLOCATE user_id_cursor;

    COMMIT TRAN;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRAN;
    THROW;
END CATCH;

