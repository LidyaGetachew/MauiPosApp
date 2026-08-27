using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPosApp.Models;
using MauiPosApp.Services;

namespace MauiPosApp.ViewModels;

public partial class OrderHistoryViewModel : ObservableObject
{
    private readonly IDatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<Order> _orders = new();

    [ObservableProperty]
    private Order? _selectedOrder;

    [ObservableProperty]
    private ObservableCollection<OrderItem> _selectedOrderItems = new();

    [ObservableProperty]
    private bool _isLoading;

    public OrderHistoryViewModel(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [RelayCommand]
    public async Task LoadOrdersAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _databaseService.GetOrdersAsync();
            Orders = new ObservableCollection<Order>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task SelectOrderAsync(Order? order)
    {
        SelectedOrder = order;
        if (order == null)
        {
            SelectedOrderItems.Clear();
            return;
        }

        var items = await _databaseService.GetOrderItemsAsync(order.Id);
        SelectedOrderItems = new ObservableCollection<OrderItem>(items);
    }
}
