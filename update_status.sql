-- Update Payment tables
UPDATE sales SET payment_method = payment_method + 1 WHERE payment_method IS NOT NULL;
UPDATE sales SET status = status + 1 WHERE status IS NOT NULL;

UPDATE sale_details SET status = status + 1 WHERE status IS NOT NULL;

UPDATE return_details SET payment_method = payment_method + 1 WHERE payment_method IS NOT NULL;
UPDATE return_details SET status = status + 1 WHERE status IS NOT NULL;

UPDATE credits SET payment_method = payment_method + 1 WHERE payment_method IS NOT NULL;
UPDATE credits SET status = status + 1 WHERE status IS NOT NULL;

UPDATE credits_details SET payment_method = payment_method + 1 WHERE payment_method IS NOT NULL;
UPDATE credits_details SET status = status + 1 WHERE status IS NOT NULL;

UPDATE recoveries SET status = status + 1 WHERE status IS NOT NULL;

-- Update Entity tables (replace 0 with 1 since EntityStatus already starts at 1, but might have 0 due to old UI logic)
UPDATE users SET status = 1 WHERE status = 0;
UPDATE customers SET status = 1 WHERE status = 0;
UPDATE products_services SET status = 1 WHERE status = 0;
UPDATE employee SET status = 1 WHERE status = 0;
