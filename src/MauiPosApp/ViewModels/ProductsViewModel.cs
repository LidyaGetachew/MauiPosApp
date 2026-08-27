using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPosApp.Models;
using MauiPosApp.Services;

namespace MauiPosApp.ViewModels;

public partial class ProductsViewModel : ObservableObject
{
    private readonly IDatabaseService _databaseService;
    private readonly ICartService _cartService;

    [ObservableProperty]
    private ObservableCollection<Product> _products = new();

    [ObservableProperty]
    private List<string> _categories = new() { "All", "Beverages", "Bakery", "Food" };

    [ObservableProperty]
    private string _selectedCategory = "All";

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _barcodeInput = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public ProductsViewModel(IDatabaseService databaseService, ICartService cartService)
    {
        _databaseService = databaseService;
        _cartService = cartService;
    }

    [RelayCommand]
    public async Task LoadProductsAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _databaseService.GetProductsAsync();
            Products = new ObservableCollection<Product>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task FilterByCategoryAsync(string category)
    {
        SelectedCategory = category;
        IsLoading = true;
        try
        {
            var list = await _databaseService.GetProductsByCategoryAsync(category);
            Products = new ObservableCollection<Product>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task SearchProductsAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _databaseService.SearchProductsAsync(SearchQuery);
            Products = new ObservableCollection<Product>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void AddToCart(Product? product)
    {
        if (product == null) return;
        _cartService.AddToCart(product, 1);
        StatusMessage = $"Added '{product.Name}' to cart.";
    }

    // Bonus Feature: Barcode scan simulation by SKU
    [RelayCommand]
    public async Task ScanBarcodeAsync()
    {
        if (string.IsNullOrWhiteSpace(BarcodeInput)) return;

        var product = await _databaseService.GetProductBySkuAsync(BarcodeInput);
        if (product != null)
        {
            _cartService.AddToCart(product, 1);
            StatusMessage = $"[Barcode Match] Added '{product.Name}' ({product.Sku}) to cart.";
            BarcodeInput = string.Empty;
        }
        else
        {
            StatusMessage = $"Barcode error: SKU '{BarcodeInput}' not found in catalog.";
        }
    }
}
