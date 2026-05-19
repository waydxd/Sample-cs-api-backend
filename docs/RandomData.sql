INSERT INTO `Books` (`CountryCode`, `Category`, `PublishDate`, `Name`)
WITH RECURSIVE seq AS (
    SELECT 1 AS value
    UNION ALL
    SELECT value + 1 FROM seq WHERE value < 1000
)
SELECT 
    -- Generates a random 2-letter country code (e.g., 'US', 'FR')
    ELT(FLOOR(RAND() * 6) + 1, 'US', 'UK', 'CA', 'TW', 'CN', 'HK') AS CountryCode,
    
    -- Generates a random category ID between 1 and 10
    FLOOR(1 + RAND() * 10) AS Category,
    
    -- Generates a random date within the last 10 years
    DATE_ADD('2016-01-01', INTERVAL FLOOR(RAND() * 3650) DAY) AS PublishDate,
    
    -- Generates a random string name like "Book_482"
    CONCAT('RndBook_', FLOOR(RAND() * 10000)) AS Name
FROM seq;