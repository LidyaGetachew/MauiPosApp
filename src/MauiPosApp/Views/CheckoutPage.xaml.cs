namespace MauiPosApp.Views;

public partial class CheckoutPage : ContentPage
{
    public CheckoutPage(ViewModels.CheckoutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
