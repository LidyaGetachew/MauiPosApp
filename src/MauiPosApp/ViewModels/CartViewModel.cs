using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPosApp.Models;
using MauiPosApp.Services;

namespace MauiPosApp.ViewModels;

public partial class CartViewModel : ObservableObject
{
    private readonly ICartService _cartService;

    public ObservableCollection<CartItem> Items => _cartService.Items;

    public decimal Subtotal => _cartService.Subtotal;
    public decimal TaxAmount => _cartService.TaxAmount;
    public decimal DiscountAmount => _cartService.DiscountAmount;
    public decimal GrandTotal => _cartService.GrandTotal;
    public bool IsEmpty => Items.Count == 0;

    [ObservableProperty]
    private decimal _discountInput;

    [ObservableProperty]
    private string _discountType = "Percent"; // Percent or Flat

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public CartViewModel(ICartService cartService)
    {
        _cartService = cartService;
        _cartService.CartChanged += OnCartChanged;
    }

    private void OnCartChanged(object? sender, EventArgs e)
    {
        NotifyCalculationsChanged();
    }

    public void NotifyCalculationsChanged()
    {
        OnPropertyChanged(nameof(Subtotal));
        OnPropertyChanged(nameof(TaxAmount));
        OnPropertyChanged(nameof(DiscountAmount));
        OnPropertyChanged(nameof(GrandTotal));
        OnPropertyChanged(nameof(IsEmpty));
    }

    [RelayCommand]
    public void IncrementQuantity(CartItem? item)
    {
        if (item == null) return;
        _cartService.IncrementQuantity(item.Product.Sku);
    }

    [RelayCommand]
    public void DecrementQuantity(CartItem? item)
    {
        if (item == null) return;
        _cartService.DecrementQuantity(item.Product.Sku);
    }

    [RelayCommand]
    public void RemoveItem(CartItem? item)
    {
        if (item == null) return;
        _cartService.RemoveFromCart(item.Product.Sku);
    }

    [RelayCommand]
    public void ApplyDiscount()
    {
        ErrorMessage = string.Empty;
        if (DiscountInput < 0)
        {
            ErrorMessage = "Discount cannot be negative.";
            return;
        }

        bool success;
        if (DiscountType == "Percent")
        {
            if (DiscountInput > 100)
            {
                ErrorMessage = "Percentage discount cannot exceed 100%.";
                return;
            }
            success = _cartService.ApplyPercentageDiscount(DiscountInput);
        }
        else
        {
            success = _cartService.ApplyFlatDiscount(DiscountInput);
        }

        if (!success)
        {
            ErrorMessage = "Invalid discount value.";
        }
    }

    [RelayCommand]
    public void ClearDiscount()
    {
        DiscountInput = 0m;
        ErrorMessage = string.Empty;
        _cartService.ClearDiscount();
    }

    [RelayCommand]
    public void ClearCart()
    {
        _cartService.ClearCart();
    }
}
