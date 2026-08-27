namespace MauiPosApp.Views;

public partial class CheckoutPage
{
    public CheckoutPage(ViewModels.CheckoutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
