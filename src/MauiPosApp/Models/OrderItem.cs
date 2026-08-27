using SQLite;

namespace MauiPosApp.Models;

[Table("OrderItems")]
public class OrderItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int OrderId { get; set; }

    public string ProductSku { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }
}
