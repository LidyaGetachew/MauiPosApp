namespace MauiPosApp.Views;

public partial class CartPage : ContentPage
{
    public CartPage(ViewModels.CartViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
