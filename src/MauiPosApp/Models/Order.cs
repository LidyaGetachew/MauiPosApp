using SQLite;

namespace MauiPosApp.Models;

[Table("Orders")]
public class Order
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed, Unique]
    public string OrderNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public decimal Subtotal { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal GrandTotal { get; set; }

    public string PaymentMethod { get; set; } = "Cash"; // Cash or Card

    public decimal CashTendered { get; set; }

    public decimal ChangeGiven { get; set; }
}
