using LeafBucket.Helpers;
using LeafBucket.Models;
using LeafBucket.Services;



namespace LeafBucket.Views.Customer;

[QueryProperty(nameof(PaymentMethod), "paymentMethod")]
public partial class OrderSummaryPage : ContentPage
{
    private Order _order = new Order();
    private readonly OrderService _orderService = new OrderService();

    public string PaymentMethod { get; set; }

    public OrderSummaryPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        double subtotalAmount = CartManager.GetTotal();
        double taxAmount = subtotalAmount * 0.10;
        double deliveryAmount = 5.00;
        double total = subtotalAmount + taxAmount + deliveryAmount;

        Name.Text =  SessionManager.UserName;
        Address.Text = SessionManager.Location;
        paymentMethod.Text = PaymentMethod;
        BindableLayout.SetItemsSource(itemsStack, CartManager.GetCartItems());

        tax.Text = $"${taxAmount:0.00}";
        deliveryFee.Text = $"${deliveryAmount:0.00}";
        subtotal.Text = $"${subtotalAmount:0.00}";
        grandTotal.Text = $"${total:0.00}";
    }

    public async void OnPlaceOrderClicked(object sender, EventArgs e) {
        double subtotalAmount = CartManager.GetTotal();
        double taxAmount = subtotalAmount * 0.10;
        double deliveryAmount = 5.00;
        double total = subtotalAmount + taxAmount + deliveryAmount;

        string orderId = Guid.NewGuid().ToString();
        _order.orderId = orderId;
        _order.customerId = SessionManager.UserId;
        _order.paymentMethod = PaymentMethod;

        _order.items = CartManager.GetCartItems().Select(i => new OrderItem
        {
            productId = i.productId,
            name = i.name,
            quantity = i.quantity,
            price = i.price,
            farmerId = i.farmerId
        }).ToList();

        _order.subtotal = CartManager.GetTotal();
        _order.farmerIds = _order.items.Select(i => i.farmerId).Distinct().ToList();
        _order.shippingAddress = SessionManager.Location;
        _order.deliveryFee = deliveryAmount;
        _order.tax = taxAmount;
        _order.total = total;
        _order.status = "Pending";
        _order.createdAt = DateTime.Now;

        try
        {
            await _orderService.placeOrder(_order);
            CartManager.ClearCart();
            await Shell.Current.GoToAsync($"OrderConfirmationPage?orderId={orderId}");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    public async void OnEditPaymentTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

  
}