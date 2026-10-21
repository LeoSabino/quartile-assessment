using Stores.Domain;

namespace Stores.UnitTests.Domain;

public class StoreTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        var companyId = Guid.NewGuid();

        var result = Store.Create(companyId, "Downtown", "US", "USD");

        Assert.True(result.IsSuccess);
        Assert.Equal(companyId, result.Value.CompanyId);
        Assert.Equal("Downtown", result.Value.Name);
        Assert.Equal("US", result.Value.CountryCode);
        Assert.Equal("USD", result.Value.CurrencyCode);
    }

    [Fact]
    public void Create_NewStoreIsActive()
    {
        var store = CreateStore();

        Assert.True(store.IsActive);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ReturnsValidationError(string? name)
    {
        var result = Store.Create(Guid.NewGuid(), name!, "US", "USD");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("Store.NameRequired", result.Error.Code);
    }

    [Fact]
    public void Create_WithEmptyCompanyId_ReturnsValidationError()
    {
        var result = Store.Create(Guid.Empty, "Downtown", "US", "USD");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("Store.CompanyIdRequired", result.Error.Code);
    }

    [Fact]
    public void Create_TrimsName()
    {
        var result = Store.Create(Guid.NewGuid(), "  Downtown  ", "US", "USD");

        Assert.Equal("Downtown", result.Value.Name);
    }

    [Fact]
    public void Create_WithNameLongerThan200_ReturnsValidationError()
    {
        var name = new string('a', 201);

        var result = Store.Create(Guid.NewGuid(), name, "US", "USD");

        Assert.False(result.IsSuccess);
        Assert.Equal("Store.NameTooLong", result.Error.Code);
    }

    [Fact]
    public void Create_WithNameOf200Chars_ReturnsSuccess()
    {
        var name = new string('a', 200);

        var result = Store.Create(Guid.NewGuid(), name, "US", "USD");

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("XX")]
    [InlineData("USA")]
    [InlineData("1A")]
    public void Create_WithInvalidCountryCode_ReturnsValidationError(string? countryCode)
    {
        var result = Store.Create(Guid.NewGuid(), "Downtown", countryCode!, "USD");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("Store.InvalidCountryCode", result.Error.Code);
    }

    [Fact]
    public void Create_NormalizesCountryCodeToUpperCase()
    {
        var result = Store.Create(Guid.NewGuid(), "Downtown", "br", "BRL");

        Assert.Equal("BR", result.Value.CountryCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("US")]
    [InlineData("ABC")]
    [InlineData("DOLLAR")]
    public void Create_WithInvalidCurrencyCode_ReturnsValidationError(string? currencyCode)
    {
        var result = Store.Create(Guid.NewGuid(), "Downtown", "US", currencyCode!);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("Store.InvalidCurrencyCode", result.Error.Code);
    }

    [Fact]
    public void Create_NormalizesCurrencyCodeToUpperCase()
    {
        var result = Store.Create(Guid.NewGuid(), "Downtown", "BR", "brl");

        Assert.Equal("BRL", result.Value.CurrencyCode);
    }

    [Fact]
    public void Update_WithValidData_ChangesStore()
    {
        var store = CreateStore();

        var result = store.Update("Uptown", "BR", "BRL");

        Assert.True(result.IsSuccess);
        Assert.Equal("Uptown", store.Name);
        Assert.Equal("BR", store.CountryCode);
        Assert.Equal("BRL", store.CurrencyCode);
    }

    [Fact]
    public void Update_WithInvalidData_DoesNotChangeStore()
    {
        var store = CreateStore();

        var result = store.Update("Uptown", "BR", "ABC");

        Assert.False(result.IsSuccess);
        Assert.Equal("Store.InvalidCurrencyCode", result.Error.Code);
        Assert.Equal("Downtown", store.Name);
        Assert.Equal("US", store.CountryCode);
        Assert.Equal("USD", store.CurrencyCode);
    }

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var store = CreateStore();

        store.Deactivate();

        Assert.False(store.IsActive);
    }

    private static Store CreateStore() =>
        Store.Create(Guid.NewGuid(), "Downtown", "US", "USD").Value;
}
