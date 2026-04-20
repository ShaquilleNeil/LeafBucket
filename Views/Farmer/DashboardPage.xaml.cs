using LeafBucket.Models;
using LeafBucket.Services;
using LeafBucket.Helpers;

namespace LeafBucket.Views.Farmer;

public partial class DashboardPage : ContentPage
{
    private readonly OrderService _orderService = new();
    private readonly ProductService _productService = new();
    private readonly ReviewService _reviewService = new();

    public DashboardPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDashboard();
    }

    private async Task LoadDashboard()
    {
        try
        {
            var name = SessionManager.UserName ?? "F";
            profileInitials.Text = name.Length > 0 ? name[0].ToString().ToUpper() : "F";

            var products = await _productService.fetchFarmerProducts();
            totalProductsLabel.Text = products?.Count.ToString() ?? "0";
            inventoryLabel.Text = products?.Sum(p => p.stockQuantity).ToString() ?? "0";

            var orders = await _orderService.fetchFarmerOrders();
            var activeOrders = orders?.Where(o =>
                o.status == "Placed" || o.status == "Preparing" || o.status == "Ready")
                .ToList();
            activeOrdersLabel.Text = activeOrders?.Count.ToString() ?? "0";

            var reviews = await _reviewService.fetchReviews(SessionManager.UserId!);
            newReviewsLabel.Text = reviews?.Count.ToString() ?? "0";

            var recent = orders?.OrderByDescending(o => o.createdAt).Take(3).ToList();

            recentActivityStack.Children.Clear();

            if (recent == null || recent.Count == 0)
            {
                noActivityLabel.IsVisible = true;
                return;
            }

            noActivityLabel.IsVisible = false;

            foreach (var order in recent)
            {
                var card = new Frame
                {
                    Padding = new Thickness(14),
                    CornerRadius = 10,
                    HasShadow = false,
                    BackgroundColor = Colors.White
                };

                var grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Auto }
                    }
                };

                var left = new StackLayout { Spacing = 2 };
                left.Children.Add(new Label
                {
                    Text = order.ShortOrderId,
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#222")
                });
                left.Children.Add(new Label
                {
                    Text = $"{order.items?.Count ?? 0} items · {order.FormattedDate}",
                    FontSize = 12,
                    TextColor = Color.FromArgb("#888")
                });

                var statusBadge = new Frame
                {
                    Padding = new Thickness(8, 4),
                    CornerRadius = 20,
                    HasShadow = false,
                    BackgroundColor = Color.FromArgb(order.StatusColor),
                    VerticalOptions = LayoutOptions.Center
                };
                statusBadge.Content = new Label
                {
                    Text = order.status,
                    FontSize = 11,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.White
                };

                grid.Add(left, 0, 0);
                grid.Add(statusBadge, 1, 0);
                card.Content = grid;

                var capturedOrder = order;
                card.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await Navigation.PushAsync(new FarmerOrderDetailsPage(capturedOrder));
                    })
                });

                recentActivityStack.Children.Add(card);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DASHBOARD ERROR: {ex.Message}");
        }
    }

    private async void OnAddProductTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new AddProductPage());
    }

    private async void OnManageProductsTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ProductsPage());
    }

    private async void OnReviewsTapped(object sender, TappedEventArgs e)
    {
        Shell.Current.CurrentItem.CurrentItem = Shell.Current.CurrentItem.Items[2];
    }
}