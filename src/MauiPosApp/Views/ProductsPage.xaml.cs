namespace MauiPosApp.Views;

public partial class ProductsPage
{
    public ProductsPage(ViewModels.ProductsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
