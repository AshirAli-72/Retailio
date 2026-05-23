-- Run this ONCE on an existing database that still has billNo / bill_No columns.
-- New installs use db_script.sql with inv_no already.

IF COL_LENGTH('sale_details', 'billNo') IS NOT NULL
    EXEC sp_rename 'sale_details.billNo', 'inv_no', 'COLUMN';

IF COL_LENGTH('return_details', 'billNo') IS NOT NULL
    EXEC sp_rename 'return_details.billNo', 'inv_no', 'COLUMN';

IF COL_LENGTH('credits', 'bill_No') IS NOT NULL
    EXEC sp_rename 'credits.bill_No', 'inv_no', 'COLUMN';
