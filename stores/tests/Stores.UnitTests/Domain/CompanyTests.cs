using Stores.Domain;

namespace Stores.UnitTests.Domain;

public class CompanyTests
{
    [Fact]
    public void Create_WithValidName_ReturnsActiveCompany()
    {
        var result = Company.Create("Contoso Retail");

        Assert.True(result.IsSuccess);
        Assert.Equal("Contoso Retail", result.Value.Name);
        Assert.True(result.Value.IsActive);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ReturnsValidationError(string? name)
    {
        var result = Company.Create(name!);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("Company.NameRequired", result.Error.Code);
    }

    [Fact]
    public void Create_WithNameLongerThan200_ReturnsValidationError()
    {
        var result = Company.Create(new string('a', 201));

        Assert.True(result.IsFailure);
        Assert.Equal("Company.NameTooLong", result.Error.Code);
    }

    [Fact]
    public void Create_TrimsName()
    {
        var result = Company.Create("  Contoso Retail  ");

        Assert.Equal("Contoso Retail", result.Value.Name);
    }

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var company = Company.Create("Contoso Retail").Value;

        company.Deactivate();

        Assert.False(company.IsActive);
    }
}
