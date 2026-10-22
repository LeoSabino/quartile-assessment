SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER FUNCTION dbo.GetProductsJson (@StoreId UNIQUEIDENTIFIER)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @Json NVARCHAR(MAX);

    SELECT @Json =
    (
        SELECT
            LOWER(CONVERT(CHAR(36), s.Id)) AS storeId,
            s.CurrencyCode AS currencyCode,
            JSON_QUERY(COALESCE((
                SELECT
                    LOWER(CONVERT(CHAR(36), p.Id)) AS id,
                    p.ProductCode AS productCode,
                    p.Name        AS name,
                    p.Description AS description,
                    p.Price       AS price,
                    p.Quantity    AS quantity,
                    p.CreatedAt   AS createdAt,
                    p.UpdatedAt   AS updatedAt
                FROM dbo.Products AS p
                WHERE p.StoreId = s.Id
                  AND p.IsActive = 1
                ORDER BY p.ProductCode
                FOR JSON PATH, INCLUDE_NULL_VALUES
            ), N'[]')) AS products
        FROM dbo.Stores AS s
        INNER JOIN dbo.Companies AS c ON c.Id = s.CompanyId
        WHERE s.Id = @StoreId
          AND s.IsActive = 1
          AND c.IsActive = 1
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    );

    RETURN @Json;
END;
GO
