
using LeafBucket.Models;

namespace LeafBucket.Views.Customer;

[QueryProperty(nameof(Order), "Order")]
public partial class OrderDetailsPage : ContentPage
{
    private Order _order;

    public Order Order
    {
        get => _order;
        set
        {
            _order = value;
            PopulateUI(_order);
        }
    }

    public OrderDetailsPage()
    {
        InitializeComponent();
    }

    private void PopulateUI(Order order)
    {
        if (order == null) return;

   
        orderIdLabel.Text = order.ShortOrderId;
        orderDateLabel.Text = order.createdAt.ToString("MMMM d, yyyy 'at' h:mm tt");

        statusLabel.Text = order.status;
        statusBadge.BackgroundColor = Color.FromArgb(order.StatusColor);

  
        UpdateProgressBar(order.status);

      
        itemsStack.Children.Clear();
        if (order.items != null)
        {
            foreach (var item in order.items)
            {
                var row = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Auto }
                    },
                    Padding = new Thickness(0, 4)
                };

                var nameLabel = new Label
                {
                    Text = item.name ?? item.productId,
                    FontSize = 14,
                    TextColor = Color.FromArgb("#333")
                };

                var qtyLabel = new Label
                {
                    Text = $"x{item.quantity}",
                    FontSize = 12,
                    TextColor = Color.FromArgb("#888")
                };

                var priceLabel = new Label
                {
                    Text = $"${item.price * item.quantity:0.00}",
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#2E7D32"),
                    VerticalOptions = LayoutOptions.Center
                };

                var nameStack = new StackLayout { Spacing = 2 };
                nameStack.Children.Add(nameLabel);
                nameStack.Children.Add(qtyLabel);

                row.Add(nameStack, 0, 0);
                row.Add(priceLabel, 1, 0);

                itemsStack.Children.Add(row);

                // Divider between items
                if (order.items.Last() != item)
                {
                    itemsStack.Children.Add(new BoxView
                    {
                        HeightRequest = 1,
                        Color = Color.FromArgb("#F0F0F0")
                    });
                }
            }
        }

      
        addressLabel.Text = order.shippingAddress;
        paymentLabel.Text = order.paymentMethod;

       
        subtotalLabel.Text = $"${order.subtotal:0.00}";
        deliveryLabel.Text = $"${order.deliveryFee:0.00}";
        taxLabel.Text = $"${order.tax:0.00}";
        totalLabel.Text = $"${order.total:0.00}";
    }

    private void UpdateProgressBar(string? status)
    {
        var active = Color.FromArgb("#2E7D32");
        var inactive = Color.FromArgb("#E0E0E0");

        stepPlaced.Color = active;
        stepPreparing.Color = status is "Preparing" or "Ready" or "Completed" ? active : inactive;
        stepReady.Color = status is "Ready" or "Completed" ? active : inactive;
        stepCompleted.Color = status == "Completed" ? active : inactive;
    }
}