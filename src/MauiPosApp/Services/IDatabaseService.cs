using MauiPosApp.Models;

namespace MauiPosApp.Services;

public interface IDatabaseService
{
    Task InitializeAsync();
    Task<List<Product>> GetProductsAsync();
    Task<List<Product>> GetProductsByCategoryAsync(string category);
    Task<List<Product>> SearchProductsAsync(string query);
    Task<Product?> GetProductBySkuAsync(string sku);
    Task<Order> SaveOrderAsync(Order order, IEnumerable<CartItem> items);
    Task<List<Order>> GetOrdersAsync();
    Task<List<OrderItem>> GetOrderItemsAsync(int orderId);
}
