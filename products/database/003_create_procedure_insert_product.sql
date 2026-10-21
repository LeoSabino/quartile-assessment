SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.InsertProduct
    @StoreId     UNIQUEIDENTIFIER,
    @ProductCode NVARCHAR(50),
    @Name        NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @Price       DECIMAL(18, 2),
    @Quantity    INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Products (StoreId, CurrencyCode, ProductCode, Name, Description, Price, Quantity)
    OUTPUT inserted.Id
    SELECT s.Id, s.CurrencyCode, @ProductCode, @Name, @Description, @Price, @Quantity
    FROM dbo.Stores AS s
    INNER JOIN dbo.Companies AS c ON c.Id = s.CompanyId
    WHERE s.Id = @StoreId
      AND s.IsActive = 1
      AND c.IsActive = 1;

    IF @@ROWCOUNT = 0
        THROW 50001, 'Store not found.', 1;
END;
GO
