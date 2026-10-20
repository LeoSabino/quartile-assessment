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
}
