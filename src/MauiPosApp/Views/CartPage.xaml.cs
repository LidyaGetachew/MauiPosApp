namespace MauiPosApp.Views;

public partial class CartPage
{
    public CartPage(ViewModels.CartViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
