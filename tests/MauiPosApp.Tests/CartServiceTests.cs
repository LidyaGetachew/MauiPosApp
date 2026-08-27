using FluentAssertions;
using MauiPosApp.Models;
using MauiPosApp.Services;

namespace MauiPosApp.Tests;

public class CartServiceTests
{
    private readonly CartService _cartService;

    public CartServiceTests()
    {
        _cartService = new CartService();
    }

    [Fact]
    public void EmptyCart_SubtotalTaxAndTotal_ShouldBeZero()
    {
        // Act & Assert
        _cartService.Subtotal.Should().Be(0m);
        _cartService.TaxAmount.Should().Be(0m);
        _cartService.DiscountAmount.Should().Be(0m);
        _cartService.GrandTotal.Should().Be(0m);
        _cartService.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddToCart_SingleProduct_CalculatesCorrectSubtotalAndTax()
    {
        // Arrange
        var espresso = new Product { Sku = "SKU-001", Name = "Espresso", Price = 2.50m, Category = "Beverages" };

        // Act
        _cartService.AddToCart(espresso, 2); // 2 * $2.50 = $5.00

        // Assert
        _cartService.Subtotal.Should().Be(5.00m);
        // Tax 8.5% of $5.00 = 0.425 -> rounded to $0.43
        _cartService.TaxAmount.Should().Be(0.43m);
        _cartService.GrandTotal.Should().Be(5.43m);
    }

    [Fact]
    public void AddToCart_MultipleProducts_Calculates8Point5PercentSalesTax()
    {
        // Arrange
        var espresso = new Product { Sku = "SKU-001", Name = "Espresso", Price = 2.50m, Category = "Beverages" };
        var croissant = new Product { Sku = "SKU-002", Name = "Croissant", Price = 3.25m, Category = "Bakery" };

        // Act
        _cartService.AddToCart(espresso, 1);   // $2.50
        _cartService.AddToCart(croissant, 2);  // $6.50
        // Subtotal = $9.00

        // Assert
        _cartService.Subtotal.Should().Be(9.00m);
        // 8.5% of $9.00 = 0.765 -> rounded to $0.77
        _cartService.TaxAmount.Should().Be(0.77m);
        _cartService.GrandTotal.Should().Be(9.77m);
    }

    [Fact]
    public void UpdateQuantity_ZeroQuantity_RemovesItemFromCart()
    {
        // Arrange
        var icedTea = new Product { Sku = "SKU-003", Name = "Iced Tea", Price = 3.00m, Category = "Beverages" };
        _cartService.AddToCart(icedTea, 2);

        // Act
        _cartService.UpdateQuantity("SKU-003", 0);

        // Assert
        _cartService.Items.Should().BeEmpty();
        _cartService.Subtotal.Should().Be(0m);
    }

    [Fact]
    public void RemoveFromCart_ExistingProduct_RemovesItem()
    {
        // Arrange
        var muffin = new Product { Sku = "SKU-004", Name = "Blueberry Muffin", Price = 3.75m, Category = "Bakery" };
        _cartService.AddToCart(muffin, 1);

        // Act
        _cartService.RemoveFromCart("SKU-004");

        // Assert
        _cartService.Items.Should().BeEmpty();
    }

    [Fact]
    public void ApplyPercentageDiscount_ValidPercentage_AppliesCorrectDiscount()
    {
        // Arrange
        var sandwich = new Product { Sku = "SKU-006", Name = "Turkey Sandwich", Price = 10.00m, Category = "Food" };
        _cartService.AddToCart(sandwich, 2); // Subtotal = $20.00

        // Act
        bool result = _cartService.ApplyPercentageDiscount(10m); // 10% of $20 = $2.00

        // Assert
        result.Should().BeTrue();
        _cartService.DiscountAmount.Should().Be(2.00m);
        _cartService.TaxableAmount.Should().Be(18.00m);
        // Tax 8.5% of $18.00 = 1.53
        _cartService.TaxAmount.Should().Be(1.53m);
        _cartService.GrandTotal.Should().Be(19.53m);
    }

    [Fact]
    public void ApplyPercentageDiscount_InvalidPercentage_ReturnsFalseAndIgnores()
    {
        // Act
        bool negativeResult = _cartService.ApplyPercentageDiscount(-5m);
        bool overResult = _cartService.ApplyPercentageDiscount(150m);

        // Assert
        negativeResult.Should().BeFalse();
        overResult.Should().BeFalse();
        _cartService.DiscountPercentage.Should().Be(0m);
    }

    [Fact]
    public void ApplyFlatDiscount_ValidAmount_AppliesCorrectDiscount()
    {
        // Arrange
        var product = new Product { Sku = "SKU-001", Name = "Espresso", Price = 5.00m, Category = "Beverages" };
        _cartService.AddToCart(product, 2); // Subtotal = $10.00

        // Act
        bool result = _cartService.ApplyFlatDiscount(3.00m);

        // Assert
        result.Should().BeTrue();
        _cartService.DiscountAmount.Should().Be(3.00m);
        _cartService.TaxableAmount.Should().Be(7.00m);
    }

    [Fact]
    public void DiscountAmount_ExceedingSubtotal_ClampsDiscountToSubtotal()
    {
        // Arrange
        var product = new Product { Sku = "SKU-005", Name = "Bottled Water", Price = 1.50m, Category = "Beverages" };
        _cartService.AddToCart(product, 1); // Subtotal = $1.50

        // Act
        _cartService.ApplyFlatDiscount(10.00m); // Attempt $10 discount on $1.50 subtotal

        // Assert
        _cartService.DiscountAmount.Should().Be(1.50m); // Clamped to $1.50
        _cartService.TaxableAmount.Should().Be(0.00m);
        _cartService.GrandTotal.Should().Be(0.00m);
    }

    [Fact]
    public void ClearCart_ResetsItemsAndTotals()
    {
        // Arrange
        var product = new Product { Sku = "SKU-001", Name = "Espresso", Price = 2.50m };
        _cartService.AddToCart(product, 2);
        _cartService.ApplyPercentageDiscount(20m);

        // Act
        _cartService.ClearCart();

        // Assert
        _cartService.Items.Should().BeEmpty();
        _cartService.Subtotal.Should().Be(0m);
        _cartService.DiscountAmount.Should().Be(0m);
        _cartService.GrandTotal.Should().Be(0m);
    }
}
