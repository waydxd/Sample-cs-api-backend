-- Create the 'Books' table if it doesn't exist
CREATE TABLE `Books` (
    `Id` BIGINT NOT NULL AUTO_INCREMENT,
    `CountryCode` varchar(5) CHARACTER SET utf8mb4 NOT NULL,
    `Category` BIGINT NOT NULL,
    `PublishDate` date NOT NULL,
    `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Books` PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Optional: Create an index on Category or CountryCode if you plan to query by them frequently
CREATE INDEX `IX_Books_Category` ON `Books` (`Category`);