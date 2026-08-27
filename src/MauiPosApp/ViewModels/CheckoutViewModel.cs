using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPosApp.Models;
using MauiPosApp.Services;

namespace MauiPosApp.ViewModels;

public partial class CheckoutViewModel : ObservableObject
{
    private readonly ICartService _cartService;
    private readonly IDatabaseService _databaseService;

    [ObservableProperty]
    private string _paymentMethod = "Cash"; // Cash or Card

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ChangeGiven))]
    private decimal _cashTendered;

    [ObservableProperty]
    private bool _isOrderConfirmed;

    [ObservableProperty]
    private Order? _completedOrder;

    [ObservableProperty]
    private string _receiptText = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public decimal Subtotal => _cartService.Subtotal;
    public decimal TaxAmount => _cartService.TaxAmount;
    public decimal DiscountAmount => _cartService.DiscountAmount;
    public decimal GrandTotal => _cartService.GrandTotal;
    public ObservableCollection<CartItem> Items => _cartService.Items;

    public decimal ChangeGiven => PaymentMethod == "Cash" && CashTendered >= GrandTotal
        ? CashTendered - GrandTotal
        : 0m;

    public CheckoutViewModel(ICartService cartService, IDatabaseService databaseService)
    {
        _cartService = cartService;
        _databaseService = databaseService;
    }

    [RelayCommand]
    public void SelectPaymentMethod(string method)
    {
        PaymentMethod = method;
        if (method == "Card")
        {
            CashTendered = GrandTotal;
        }
    }

    [RelayCommand]
    public async Task ConfirmCheckoutAsync()
    {
        StatusMessage = string.Empty;

        if (Items.Count == 0)
        {
            StatusMessage = "Cannot checkout with an empty cart.";
            return;
        }

        if (PaymentMethod == "Cash" && CashTendered < GrandTotal)
        {
            StatusMessage = $"Insufficient cash tendered. Total is ${GrandTotal:F2}.";
            return;
        }

        var order = new Order
        {
            Subtotal = Subtotal,
            TaxAmount = TaxAmount,
            DiscountAmount = DiscountAmount,
            GrandTotal = GrandTotal,
            PaymentMethod = PaymentMethod,
            CashTendered = PaymentMethod == "Cash" ? CashTendered : GrandTotal,
            ChangeGiven = ChangeGiven
        };

        // Persist to local SQLite database
        var savedOrder = await _databaseService.SaveOrderAsync(order, Items);
        CompletedOrder = savedOrder;
        IsOrderConfirmed = true;

        // Generate receipt text
        ReceiptText = GenerateReceipt(savedOrder, Items);

        // Clear active cart state
        _cartService.ClearCart();
    }

    public string GenerateReceipt(Order order, IEnumerable<CartItem> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine("========================================");
        sb.AppendLine("         MAUI CAFE & POS RECEIPT       ");
        sb.AppendLine("========================================");
        sb.AppendLine($"Order Number: {order.OrderNumber}");
        sb.AppendLine($"Date & Time:  {order.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Payment:      {order.PaymentMethod}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine(string.Format("{0,-20} {1,5} {2,12}", "ITEM", "QTY", "TOTAL"));
        sb.AppendLine("----------------------------------------");

        foreach (var item in items)
        {
            sb.AppendLine(string.Format("{0,-20} {1,5} ${2,11:F2}", item.Product.Name, item.Quantity, item.Subtotal));
        }

        sb.AppendLine("----------------------------------------");
        sb.AppendLine($"Subtotal:                ${order.Subtotal:F2}");
        if (order.DiscountAmount > 0)
        {
            sb.AppendLine($"Discount:               -${order.DiscountAmount:F2}");
        }
        sb.AppendLine($"Sales Tax (8.5%):        ${order.TaxAmount:F2}");
        sb.AppendLine($"GRAND TOTAL:             ${order.GrandTotal:F2}");
        if (order.PaymentMethod == "Cash")
        {
            sb.AppendLine($"Cash Tendered:           ${order.CashTendered:F2}");
            sb.AppendLine($"Change Given:            ${order.ChangeGiven:F2}");
        }
        sb.AppendLine("========================================");
        sb.AppendLine("       Thank you for your visit!        ");
        sb.AppendLine("========================================");

        return sb.ToString();
    }

    [RelayCommand]
    public void ResetCheckout()
    {
        IsOrderConfirmed = false;
        CompletedOrder = null;
        ReceiptText = string.Empty;
        CashTendered = 0m;
        StatusMessage = string.Empty;
    }
}
