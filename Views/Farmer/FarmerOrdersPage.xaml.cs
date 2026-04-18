using LeafBucket.Components;
using LeafBucket.Services;
using LeafBucket.ViewModels.Farmer;
using System.Threading.Tasks;

namespace LeafBucket.Views.Farmer;

public partial class FarmerOrdersPage : ContentPage
{
    OrderService orderService = new OrderService();
    public FarmerOrdersPage()
	{
		InitializeComponent();
        MessagingCenter.Subscribe<FarmerOrderCard>(this, "OrderStatusChanged", async (sender) =>
        {
            await LoadOrders();
        });

        MessagingCenter.Subscribe<FarmerOrderDetailsPage>(this, "OrderStatusChanged", async (sender) =>
        {
            await LoadOrders();
        });
    }

	protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadOrders();

    }

    public async Task LoadOrders()
    {
        try {
            var orders = await orderService.fetchFarmerOrders();


            if (orders == null || orders.Count == 0)
            {
                emptyState.IsVisible = true;
                ordersStack.IsVisible = false;
                return;
            }

            emptyState.IsVisible = false;
            ordersStack.IsVisible = true;

            var sorted = orders.OrderByDescending(o => o.createdAt).ToList();
            BindableLayout.SetItemsSource(ordersStack, sorted);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load orders: {ex.Message}", "OK");
            return;
        }
       


    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadOrders();
        refreshView.IsRefreshing = false;
    }

    private void OnSearchChanged(object sender, TextChangedEventArgs e)
    {

    }
}


