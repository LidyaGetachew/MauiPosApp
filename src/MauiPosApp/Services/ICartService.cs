using System.Collections.ObjectModel;
using MauiPosApp.Models;

namespace MauiPosApp.Services;

public interface ICartService
{
    ObservableCollection<CartItem> Items { get; }
    decimal SalesTaxRate { get; }
    decimal Subtotal { get; }
    decimal TaxAmount { get; }
    decimal DiscountAmount { get; }
    decimal GrandTotal { get; }
    decimal DiscountPercentage { get; }
    decimal FlatDiscount { get; }

    event EventHandler? CartChanged;

    void AddToCart(Product product, int quantity = 1);
    void UpdateQuantity(string sku, int quantity);
    void IncrementQuantity(string sku);
    void DecrementQuantity(string sku);
    void RemoveFromCart(string sku);
    bool ApplyPercentageDiscount(decimal percentage);
    bool ApplyFlatDiscount(decimal amount);
    void ClearDiscount();
    void ClearCart();
}
