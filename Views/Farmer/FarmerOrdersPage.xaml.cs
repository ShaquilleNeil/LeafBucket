using LeafBucket.Components;
using LeafBucket.Helpers;
using LeafBucket.Models;
using LeafBucket.Services;
using LeafBucket.ViewModels.Farmer;
using System.Threading.Tasks;

namespace LeafBucket.Views.Farmer;

public partial class FarmerOrdersPage : ContentPage
{
    OrderService orderService = new OrderService();
    private List<Order> _allOrders = new();

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
        try
        {
            var orders = await orderService.fetchFarmerOrders();

            if (orders == null || orders.Count == 0)
            {
                emptyState.IsVisible = true;
                ordersStack.IsVisible = false;
                return;
            }

            var myOrders = orders.Select(o => new Order
            {
                orderId = o.orderId,
                customerId = o.customerId,
                status = o.status,
                createdAt = o.createdAt,
                shippingAddress = o.shippingAddress,
                paymentMethod = o.paymentMethod,
                farmerIds = o.farmerIds,
                deliveryFee = o.deliveryFee,
                tax = o.tax,
                items = o.items?
                    .Where(i => i.farmerId == SessionManager.UserId)
                    .ToList(),
                subtotal = o.items?
                    .Where(i => i.farmerId == SessionManager.UserId)
                    .Sum(i => i.price * i.quantity) ?? 0,
                total = o.items?
                    .Where(i => i.farmerId == SessionManager.UserId)
                    .Sum(i => i.price * i.quantity) ?? 0
            })
            .Where(o => o.items != null && o.items.Count > 0)
            .ToList();

            if (myOrders.Count == 0)
            {
                emptyState.IsVisible = true;
                ordersStack.IsVisible = false;
                return;
            }

            emptyState.IsVisible = false;
            ordersStack.IsVisible = true;

            var sorted = myOrders.OrderByDescending(o => o.createdAt).ToList();
            _allOrders = sorted;
            BindableLayout.SetItemsSource(ordersStack, sorted);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load orders: {ex.Message}", "OK");
        }
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadOrders();
        refreshView.IsRefreshing = false;
    }

    private void OnSearchChanged(object sender, TextChangedEventArgs e)
    {
        var query = e.NewTextValue?.ToLower() ?? "";

        if (string.IsNullOrWhiteSpace(query))
        {
            BindableLayout.SetItemsSource(ordersStack, _allOrders);
            return;
        }

        var filtered = _allOrders.Where(o =>
            o.items != null &&
            o.items.Any(i => i.name?.ToLower().Contains(query) == true)
        ).ToList();

        BindableLayout.SetItemsSource(ordersStack, filtered);

    }
}


