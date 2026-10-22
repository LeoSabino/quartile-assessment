using Products.Functions.Domain;

namespace Products.UnitTests.Domain;

public class ProductTests
{
    private static readonly Guid StoreId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        var result = Product.Create(StoreId, "123", "Coffee", "Arabica 500g", 10.50m, 3);

        Assert.True(result.IsSuccess);
        Assert.Equal(StoreId, result.Value.StoreId);
        Assert.Equal("123", result.Value.ProductCode);
        Assert.Equal("Coffee", result.Value.Name);
        Assert.Equal("Arabica 500g", result.Value.Description);
        Assert.Equal(10.50m, result.Value.Price);
        Assert.Equal(3, result.Value.Quantity);
    }

    [Fact]
    public void Create_NewProductIsActive()
    {
        var product = CreateProduct();

        Assert.True(product.IsActive);
    }

    [Fact]
    public void Create_WithEmptyStoreId_ReturnsValidationError()
    {
        var result = Product.Create(Guid.Empty, "123", "Coffee", null, 10m, 0);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("Product.StoreIdRequired", result.Error.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyProductCode_ReturnsValidationError(string? productCode)
    {
        var result = Product.Create(StoreId, productCode!, "Coffee", null, 10m, 0);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("Product.ProductCodeRequired", result.Error.Code);
    }

    [Fact]
    public void Create_WithProductCodeLongerThan50_ReturnsValidationError()
    {
        var result = Product.Create(StoreId, new string('A', 51), "Coffee", null, 10m, 0);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.ProductCodeTooLong", result.Error.Code);
    }

    [Fact]
    public void Create_WithProductCodeOf50Chars_ReturnsSuccess()
    {
        var result = Product.Create(StoreId, new string('A', 50), "Coffee", null, 10m, 0);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ReturnsValidationError(string? name)
    {
        var result = Product.Create(StoreId, "123", name!, null, 10m, 0);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("Product.NameRequired", result.Error.Code);
    }

    [Fact]
    public void Create_WithNameLongerThan200_ReturnsValidationError()
    {
        var result = Product.Create(StoreId, "123", new string('a', 201), null, 10m, 0);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.NameTooLong", result.Error.Code);
    }

    [Fact]
    public void Create_WithNameOf200Chars_ReturnsSuccess()
    {
        var result = Product.Create(StoreId, "123", new string('a', 200), null, 10m, 0);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_WithDescriptionLongerThan1000_ReturnsValidationError()
    {
        var result = Product.Create(StoreId, "123", "Coffee", new string('a', 1001), 10m, 0);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.DescriptionTooLong", result.Error.Code);
    }

    [Fact]
    public void Create_WithDescriptionOf1000Chars_ReturnsSuccess()
    {
        var result = Product.Create(StoreId, "123", "Coffee", new string('a', 1000), 10m, 0);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyDescription_StoresNull(string? description)
    {
        var result = Product.Create(StoreId, "123", "Coffee", description, 10m, 0);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Description);
    }

    [Fact]
    public void Create_TrimsTextFields()
    {
        var result = Product.Create(StoreId, "  ABC-1  ", "  Coffee  ", "  Arabica  ", 10m, 0);

        Assert.Equal("ABC-1", result.Value.ProductCode);
        Assert.Equal("Coffee", result.Value.Name);
        Assert.Equal("Arabica", result.Value.Description);
    }

    [Fact]
    public void Create_WithNegativePrice_ReturnsValidationError()
    {
        var result = Product.Create(StoreId, "ABC-1", "Coffee", null, -0.01m, 0);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("Product.InvalidPrice", result.Error.Code);
    }

    [Fact]
    public void Create_WithMoreThanTwoDecimalPlaces_ReturnsValidationError()
    {
        var result = Product.Create(StoreId, "ABC-1", "Coffee", null, 10.555m, 0);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.InvalidPrice", result.Error.Code);
    }

    [Fact]
    public void Create_WithZeroPriceAndQuantity_ReturnsSuccess()
    {
        var result = Product.Create(StoreId, "ABC-1", "Coffee", null, 0m, 0);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_WithNegativeQuantity_ReturnsValidationError()
    {
        var result = Product.Create(StoreId, "ABC-1", "Coffee", null, 10m, -1);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("Product.InvalidQuantity", result.Error.Code);
    }

    [Fact]
    public void Update_WithValidData_ChangesProduct()
    {
        var product = CreateProduct();

        var result = product.Update("ABC-1", "Espresso", "Dark roast", 12.90m, 7);

        Assert.True(result.IsSuccess);
        Assert.Equal("Espresso", product.Name);
        Assert.Equal("Dark roast", product.Description);
        Assert.Equal(12.90m, product.Price);
        Assert.Equal(7, product.Quantity);
    }

    [Fact]
    public void Update_WithDifferentProductCode_ReturnsValidationError()
    {
        var product = CreateProduct();

        var result = product.Update("XYZ-9", "Espresso", null, 12.90m, 7);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("Product.ProductCodeCannotChange", result.Error.Code);
        Assert.Equal("ABC-1", product.ProductCode);
        Assert.Equal("Coffee", product.Name);
    }

    [Fact]
    public void Update_WithProductCodeInDifferentCase_KeepsOriginalCode()
    {
        var product = CreateProduct();

        var result = product.Update("abc-1", "Espresso", null, 12.90m, 7);

        Assert.True(result.IsSuccess);
        Assert.Equal("ABC-1", product.ProductCode);
    }

    [Fact]
    public void Update_WithInvalidData_DoesNotChangeProduct()
    {
        var product = CreateProduct();

        var result = product.Update("ABC-1", "Espresso", null, -1m, 7);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.InvalidPrice", result.Error.Code);
        Assert.Equal("Coffee", product.Name);
        Assert.Equal(10.50m, product.Price);
        Assert.Equal(3, product.Quantity);
    }

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var product = CreateProduct();

        product.Deactivate();

        Assert.False(product.IsActive);
    }

    private static Product CreateProduct() =>
        Product.Create(StoreId, "ABC-1", "Coffee", "Arabica 500g", 10.50m, 3).Value;
}
