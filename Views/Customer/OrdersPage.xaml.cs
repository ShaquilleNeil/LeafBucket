using LeafBucket.Services;
using System.Threading.Tasks;

namespace LeafBucket.Views.Customer;

public partial class OrdersPage : ContentPage
{
    private readonly OrderService _orderService = new OrderService();
    private List<LeafBucket.Models.Order> _allOrders = new();

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
        refreshView.IsRefreshing = false;
    }

    private async Task LoadOrders()
    {
        try
        {
            var orders = await _orderService.fetchCustomerOrders();

            if (orders == null || orders.Count == 0)
            {
                emptyState.IsVisible = true;
                ordersStack.IsVisible = false;
                return;
            }

            emptyState.IsVisible = false;
            ordersStack.IsVisible = true;

            var sorted = orders.OrderByDescending(o => o.createdAt).ToList();
            _allOrders = sorted;
            BindableLayout.SetItemsSource(ordersStack, sorted);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load orders: {ex.Message}", "OK");
        }
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