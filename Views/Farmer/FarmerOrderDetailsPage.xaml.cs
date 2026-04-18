using LeafBucket.ViewModels.Farmer;
using LeafBucket.Models;
using System.Threading.Tasks;
using LeafBucket.Services;

namespace LeafBucket.Views.Farmer;


public partial class FarmerOrderDetailsPage : ContentPage
{
	private Order _order;
    private readonly OrderService _orderService = new OrderService();
	public FarmerOrderDetailsPage(Order order)
	{
		InitializeComponent();
		_order = order;
		LoadElements();


       
	}


	public void LoadElements() {


        foreach (var item in _order.items ?? new())
        {
            Console.WriteLine($"ITEM: name={item.name}, qty={item.quantity}, price={item.price}");
        }

        orderIdLabel.Text = _order.ShortOrderId;
        statusLabel.Text = _order.status;
		orderDateLabel.Text = _order.FormattedDate;
		customerAddressLabel.Text = _order.shippingAddress;
		itemsCountLabel.Text = $"Items ({_order.items?.Count ?? 0})";
        statusBadge.BackgroundColor = Color.FromArgb(_order.StatusColor);

        subtotalLabel.Text = $"${_order.subtotal:0.00}";
        deliveryLabel.Text = $"${_order.deliveryFee:0.00}";
        totalLabel.Text = $"${_order.total:0.00}";

        BindableLayout.SetItemsSource(itemsStack, _order.items);
        SetupActionButton();

    }

	public async void OnActionButtonClicked(object sender, EventArgs e) {


        var status = _order.status;

        switch (status)
        {
            case "Placed":
                actionButton.Text = "ACCEPT ORDER";
                actionButton.BackgroundColor = Color.FromArgb("#2E7D32");
                actionButton.IsVisible = true;
                break;
            case "Preparing":
                actionButton.Text = "MARK AS SHIPPED";
                actionButton.BackgroundColor = Color.FromArgb("#E65100");
                actionButton.IsVisible = true;
                break;
            case "Shipped":
                actionButton.Text = "MARK AS COMPLETED";
                actionButton.BackgroundColor = Color.FromArgb("#558B2F");
                actionButton.IsVisible = true;
                break;
            case "Completed":
            case "Cancelled":
            default:
                actionButton.IsVisible = false;
                break;
        }

        MessagingCenter.Send(this, "OrderStatusChanged");
        await Navigation.PopAsync();
    }

	public void SetupActionButton() {

		var status = _order.status;

		switch (status) {
			case "Placed":
				actionButton.Text = "Accept Order";
				break;
			case "Preparing":
				actionButton.Text = "Mark As Shipped";
				break;
			case "Shipped":
				actionButton.Text = "Mark as Completed";
				break;
            case "Completed":
            case "Cancelled":
                actionButton.IsVisible = false;
                break;
			default:
				actionButton.IsVisible = false;
				break;

        }
	}

}



