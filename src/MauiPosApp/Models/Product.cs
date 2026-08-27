using SQLite;

namespace MauiPosApp.Models;

[Table("Products")]
public class Product
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed, Unique]
    public string Sku { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
}
