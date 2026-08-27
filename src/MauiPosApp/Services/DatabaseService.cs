using MauiPosApp.Models;
using SQLite;

namespace MauiPosApp.Services;

public class DatabaseService : IDatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DatabaseService(string? dbPath = null)
    {
        _dbPath = dbPath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mauipos.db3");
    }

    public async Task InitializeAsync()
    {
        if (_database != null) return;

        var dbDir = Path.GetDirectoryName(_dbPath);
        if (!string.IsNullOrEmpty(dbDir) && !Directory.Exists(dbDir))
        {
            Directory.CreateDirectory(dbDir);
        }

        _database = new SQLiteAsyncConnection(_dbPath);

        await _database.CreateTableAsync<Product>();
        await _database.CreateTableAsync<Order>();
        await _database.CreateTableAsync<OrderItem>();

        await SeedProductsAsync();
    }

    private async Task SeedProductsAsync()
    {
        if (_database == null) return;

        var count = await _database.Table<Product>().CountAsync();
        if (count == 0)
        {
            var seedProducts = new List<Product>
            {
                new() { Sku = "SKU-001", Name = "Espresso", Price = 2.50m, Category = "Beverages" },
                new() { Sku = "SKU-002", Name = "Croissant", Price = 3.25m, Category = "Bakery" },
                new() { Sku = "SKU-003", Name = "Iced Tea", Price = 3.00m, Category = "Beverages" },
                new() { Sku = "SKU-004", Name = "Blueberry Muffin", Price = 3.75m, Category = "Bakery" },
                new() { Sku = "SKU-005", Name = "Bottled Water", Price = 1.50m, Category = "Beverages" },
                new() { Sku = "SKU-006", Name = "Turkey Sandwich", Price = 6.95m, Category = "Food" }
            };

            await _database.InsertAllAsync(seedProducts);
        }
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        await InitializeAsync();
        return await _database!.Table<Product>().ToListAsync();
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(string category)
    {
        await InitializeAsync();
        if (string.IsNullOrWhiteSpace(category) || category.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            return await GetProductsAsync();
        }
        return await _database!.Table<Product>().Where(p => p.Category == category).ToListAsync();
    }

    public async Task<List<Product>> SearchProductsAsync(string query)
    {
        await InitializeAsync();
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetProductsAsync();
        }
        var lowerQuery = query.Trim().ToLower();
        return await _database!.Table<Product>()
            .Where(p => p.Name.ToLower().Contains(lowerQuery) || p.Sku.ToLower().Contains(lowerQuery) || p.Category.ToLower().Contains(lowerQuery))
            .ToListAsync();
    }

    public async Task<Product?> GetProductBySkuAsync(string sku)
    {
        await InitializeAsync();
        if (string.IsNullOrWhiteSpace(sku)) return null;
        var normalized = sku.Trim().ToUpper();
        return await _database!.Table<Product>().FirstOrDefaultAsync(p => p.Sku == normalized);
    }

    public async Task<Order> SaveOrderAsync(Order order, IEnumerable<CartItem> items)
    {
        await InitializeAsync();

        if (string.IsNullOrEmpty(order.OrderNumber))
        {
            var count = await _database!.Table<Order>().CountAsync();
            order.OrderNumber = $"ORD-{(count + 1):D5}";
        }

        order.CreatedAt = DateTime.UtcNow;
        await _database!.InsertAsync(order);

        var orderItems = items.Select(item => new OrderItem
        {
            OrderId = order.Id,
            ProductSku = item.Product.Sku,
            ProductName = item.Product.Name,
            UnitPrice = item.Product.Price,
            Quantity = item.Quantity,
            TotalPrice = item.Subtotal
        }).ToList();

        await _database.InsertAllAsync(orderItems);
        return order;
    }

    public async Task<List<Order>> GetOrdersAsync()
    {
        await InitializeAsync();
        return await _database!.Table<Order>().OrderByDescending(o => o.CreatedAt).ToListAsync();
    }

    public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
    {
        await InitializeAsync();
        return await _database!.Table<OrderItem>().Where(oi => oi.OrderId == orderId).ToListAsync();
    }
}
