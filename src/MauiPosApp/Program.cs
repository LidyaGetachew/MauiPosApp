namespace MauiPosApp;

public class Program
{
    public static void Main(string[] args)
    {
        var app = MauiProgram.CreateMauiApp();
        Console.WriteLine("==================================================");
        Console.WriteLine(" .NET MAUI POS Application - Cashier Engine       ");
        Console.WriteLine("==================================================");
        Console.WriteLine("Initialization complete. Registered DI Services:");
        Console.WriteLine(" - IDatabaseService -> DatabaseService (SQLite)");
        Console.WriteLine(" - ICartService -> CartService (Tax 8.5%)");
        Console.WriteLine(" - ProductsViewModel / CartViewModel / CheckoutViewModel");
        Console.WriteLine("Ready for Cashier POS Operations.");
        Console.WriteLine("==================================================");
    }
}
