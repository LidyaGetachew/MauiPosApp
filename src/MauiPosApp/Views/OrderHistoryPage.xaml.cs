namespace MauiPosApp.Views;

public partial class OrderHistoryPage : ContentPage
{
    public OrderHistoryPage(ViewModels.OrderHistoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
