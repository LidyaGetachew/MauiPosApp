using FluentAssertions;
using MauiPosApp.Legacy;
using MauiPosApp.Models;

namespace MauiPosApp.Tests;

public class LegacyCodeFixTests
{
    [Fact]
    public void GetTotal_WithPercentageDiscount_FixesUnitBug()
    {
        // Arrange
        var vm = new FixedCartViewModel();
        var product = new Product { Sku = "SKU-001", Name = "Espresso", Price = 10.00m };
        vm.Items.Add(new CartItem(product, 1)); // Subtotal = $10.00

        // Act
        vm.ApplyDiscount(15m); // 15% discount -> Should subtract $1.50, Total = $8.50

        // Assert
        vm.Total.Should().Be(8.50m);
    }

    [Fact]
    public void ApplyDiscount_InvalidPercentage_ThrowsException()
    {
        // Arrange
        var vm = new FixedCartViewModel();

        // Act & Assert
        Action actNegative = () => vm.ApplyDiscount(-10m);
        Action actExcess = () => vm.ApplyDiscount(110m);

        actNegative.Should().Throw<ArgumentOutOfRangeException>();
        actExcess.Should().Throw<ArgumentOutOfRangeException>();
    }
}
