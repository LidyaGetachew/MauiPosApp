using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiPosApp.Models;

public partial class CartItem : ObservableObject
{
    public Product Product { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Subtotal))]
    private int _quantity;

    public CartItem(Product product, int quantity = 1)
    {
        Product = product ?? throw new ArgumentNullException(nameof(product));
        _quantity = Math.Max(1, quantity);
    }

    public decimal Subtotal => Product.Price * Quantity;
}
