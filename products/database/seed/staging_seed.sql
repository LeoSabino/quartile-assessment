-- Staging only: never run against production.
DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-0000000000c1';
DECLARE @StoreBrId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-0000000000b1';
DECLARE @StoreUsId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-0000000000b2';

IF NOT EXISTS (SELECT 1 FROM dbo.Companies WHERE Id = @CompanyId)
    INSERT INTO dbo.Companies (Id, Name, IsActive)
    VALUES (@CompanyId, N'Staging Company', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Stores WHERE Id = @StoreBrId)
    INSERT INTO dbo.Stores (Id, CompanyId, Name, CountryCode, CurrencyCode, IsActive)
    VALUES (@StoreBrId, @CompanyId, N'Staging Store Brazil', 'BR', 'BRL', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Stores WHERE Id = @StoreUsId)
    INSERT INTO dbo.Stores (Id, CompanyId, Name, CountryCode, CurrencyCode, IsActive)
    VALUES (@StoreUsId, @CompanyId, N'Staging Store US', 'US', 'USD', 1);
