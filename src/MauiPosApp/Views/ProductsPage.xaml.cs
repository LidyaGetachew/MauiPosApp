namespace MauiPosApp.Views;

public partial class ProductsPage : ContentPage
{
    public ProductsPage(ViewModels.ProductsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
