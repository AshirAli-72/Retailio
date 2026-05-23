-- Convert legacy SALE_n / RETURN_n values to inv001 format
UPDATE sale_details
SET inv_no = 'inv' + RIGHT('000' + CAST(
    TRY_CAST(
        SUBSTRING(inv_no, CHARINDEX('_', inv_no) + 1, 50)
    AS INT) AS VARCHAR(10)), 3)
WHERE inv_no IS NOT NULL
  AND (inv_no LIKE 'SALE[_]%' OR inv_no LIKE 'RETURN[_]%')
  AND CHARINDEX('_', inv_no) > 0;

UPDATE return_details
SET inv_no = 'inv' + RIGHT('000' + CAST(
    TRY_CAST(
        SUBSTRING(inv_no, CHARINDEX('_', inv_no) + 1, 50)
    AS INT) AS VARCHAR(10)), 3)
WHERE inv_no IS NOT NULL
  AND (inv_no LIKE 'SALE[_]%' OR inv_no LIKE 'RETURN[_]%')
  AND CHARINDEX('_', inv_no) > 0;

UPDATE credits
SET inv_no = 'inv' + RIGHT('000' + CAST(
    TRY_CAST(
        SUBSTRING(inv_no, CHARINDEX('_', inv_no) + 1, 50)
    AS INT) AS VARCHAR(10)), 3)
WHERE inv_no IS NOT NULL
  AND (inv_no LIKE 'SALE[_]%' OR inv_no LIKE 'RETURN[_]%')
  AND CHARINDEX('_', inv_no) > 0;
