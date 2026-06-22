-- Fix corrupted product ImageUrl values caused by Path.GetFileName() on picsum URLs
-- The old code extracted just "400" from "https://picsum.photos/seed/xxx/400/400" and stored it.
-- After this fix, products with no valid image will show a placeholder.
-- Admin can re-upload images via the Edit Product page.

UPDATE Products
SET ImageUrl = ''
WHERE ImageUrl IS NOT NULL
  AND ImageUrl != ''
  AND ImageUrl NOT LIKE 'http%'
  AND ImageUrl NOT LIKE '/%'
  AND ImageUrl NOT LIKE '%.%'  -- no file extension = corrupted
  AND PATINDEX('%[^0-9]%', ImageUrl) = 0  -- only digits
