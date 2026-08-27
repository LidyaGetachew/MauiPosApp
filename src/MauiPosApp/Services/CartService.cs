using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using MauiPosApp.Models;

namespace MauiPosApp.Services;

public class CartService : ICartService
{
    public ObservableCollection<CartItem> Items { get; } = new();

    public decimal SalesTaxRate => 0.085m; // 8.5%

    public decimal DiscountPercentage { get; private set; }
    public decimal FlatDiscount { get; private set; }

    public event EventHandler? CartChanged;

    public CartService()
    {
        Items.CollectionChanged += OnItemsCollectionChanged;
    }

    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (CartItem item in e.NewItems)
            {
                item.PropertyChanged += OnItemPropertyChanged;
            }
        }
        if (e.OldItems != null)
        {
            foreach (CartItem item in e.OldItems)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
            }
        }
        NotifyCartChanged();
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CartItem.Quantity) || e.PropertyName == nameof(CartItem.Subtotal))
        {
            NotifyCartChanged();
        }
    }

    public decimal Subtotal => Items.Sum(i => i.Subtotal);

    public decimal DiscountAmount
    {
        get
        {
            if (Subtotal <= 0) return 0m;

            decimal discount = 0m;
            if (DiscountPercentage > 0)
            {
                discount = Subtotal * (DiscountPercentage / 100m);
            }
            else if (FlatDiscount > 0)
            {
                discount = FlatDiscount;
            }

            // Clamped so total discount never exceeds subtotal (can't go below $0)
            return Math.Min(Subtotal, Math.Round(discount, 2, MidpointRounding.AwayFromZero));
        }
    }

    public decimal TaxableAmount => Math.Max(0m, Subtotal - DiscountAmount);

    public decimal TaxAmount => Math.Round(TaxableAmount * SalesTaxRate, 2, MidpointRounding.AwayFromZero);

    public decimal GrandTotal => TaxableAmount + TaxAmount;

    public void AddToCart(Product product, int quantity = 1)
    {
        if (product == null || quantity <= 0) return;

        var existingItem = Items.FirstOrDefault(i => i.Product.Sku == product.Sku);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            Items.Add(new CartItem(product, quantity));
        }
        NotifyCartChanged();
    }

    public void UpdateQuantity(string sku, int quantity)
    {
        var existingItem = Items.FirstOrDefault(i => i.Product.Sku == sku);
        if (existingItem == null) return;

        if (quantity <= 0)
        {
            Items.Remove(existingItem);
        }
        else
        {
            existingItem.Quantity = quantity;
        }
        NotifyCartChanged();
    }

    public void IncrementQuantity(string sku)
    {
        var existingItem = Items.FirstOrDefault(i => i.Product.Sku == sku);
        if (existingItem != null)
        {
            existingItem.Quantity++;
            NotifyCartChanged();
        }
    }

    public void DecrementQuantity(string sku)
    {
        var existingItem = Items.FirstOrDefault(i => i.Product.Sku == sku);
        if (existingItem != null)
        {
            if (existingItem.Quantity > 1)
            {
                existingItem.Quantity--;
            }
            else
            {
                Items.Remove(existingItem);
            }
            NotifyCartChanged();
        }
    }

    public void RemoveFromCart(string sku)
    {
        var existingItem = Items.FirstOrDefault(i => i.Product.Sku == sku);
        if (existingItem != null)
        {
            Items.Remove(existingItem);
            NotifyCartChanged();
        }
    }

    public bool ApplyPercentageDiscount(decimal percentage)
    {
        if (percentage < 0 || percentage > 100) return false;

        DiscountPercentage = percentage;
        FlatDiscount = 0m;
        NotifyCartChanged();
        return true;
    }

    public bool ApplyFlatDiscount(decimal amount)
    {
        if (amount < 0) return false;

        FlatDiscount = amount;
        DiscountPercentage = 0m;
        NotifyCartChanged();
        return true;
    }

    public void ClearDiscount()
    {
        DiscountPercentage = 0m;
        FlatDiscount = 0m;
        NotifyCartChanged();
    }

    public void ClearCart()
    {
        Items.Clear();
        ClearDiscount();
    }

    private void NotifyCartChanged()
    {
        CartChanged?.Invoke(this, EventArgs.Empty);
    }
}
