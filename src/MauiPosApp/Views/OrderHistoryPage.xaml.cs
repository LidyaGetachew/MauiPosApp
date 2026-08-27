namespace MauiPosApp.Views;

public partial class OrderHistoryPage
{
    public OrderHistoryPage(ViewModels.OrderHistoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
