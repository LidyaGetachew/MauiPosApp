using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPosApp.Models;

namespace MauiPosApp.Legacy;

public partial class FixedCartViewModel : ObservableObject
{
    public ObservableCollection<CartItem> Items { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Total))]
    private decimal _discountPercent;

    public FixedCartViewModel()
    {
        Items.CollectionChanged += OnItemsCollectionChanged;
    }

    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Total));
    }

    public decimal Total
    {
        get
        {
            decimal subtotal = Items.Sum(item => item.Subtotal);
            if (subtotal <= 0) return 0m;

            // Fix 1: Divide discount percentage by 100 to get correct decimal fraction
            // Fix 2: Clamp percentage between 0% and 100%
            decimal clampedDiscount = Math.Clamp(DiscountPercent, 0m, 100m);
            decimal discountAmount = subtotal * (clampedDiscount / 100m);

            return Math.Max(0m, Math.Round(subtotal - discountAmount, 2, MidpointRounding.AwayFromZero));
        }
    }

    [RelayCommand]
    public void ApplyDiscount(decimal percent)
    {
        // Fix 3: Validation and property notification trigger
        if (percent < 0m || percent > 100m)
            throw new ArgumentOutOfRangeException(nameof(percent), "Discount percentage must be between 0 and 100.");

        DiscountPercent = percent;
    }
}
