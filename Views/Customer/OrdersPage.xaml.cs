using LeafBucket.ViewModels.Customer;
using System.Threading.Tasks;
using LeafBucket.Services;

namespace LeafBucket.Views.Customer;

public partial class OrdersPage : ContentPage
{
    OrderService _orderService = new OrderService();

    public OrdersPage()
	{
		InitializeComponent();
       
	}

	protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadOrders();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadOrders();
    }

    private async Task LoadOrders() {
        try
        {
            var orders = await _orderService.fetchCustomerOrders();

            if (orders == null || orders.Count == 0) {
                emptyState.IsVisible = true;
                ordersStack.IsVisible = false;
                return;
            }

            emptyState.IsVisible = false;
            ordersStack.IsVisible = true;

            var sorted = orders.OrderByDescending(o => o.createdAt).ToList();
            BindableLayout.SetItemsSource(ordersStack, sorted);

        }
        catch (Exception ex) {
            
            await DisplayAlert("Error", $"Failed to load orders: {ex.Message}", "OK");
        }
    }
}


