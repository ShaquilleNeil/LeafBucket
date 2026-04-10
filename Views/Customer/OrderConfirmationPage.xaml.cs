namespace LeafBucket.Views.Customer;

[QueryProperty(nameof(OrderId), "orderId")]
public partial class OrderConfirmationPage : ContentPage
{
    private string _orderId;

    public string OrderId
    {
        get => _orderId;
        set
        {
            _orderId = value;
            orderId.Text = $"Order #{_orderId}";
        }
    }

    public OrderConfirmationPage()
    {
        InitializeComponent();
    }

    public async void OnViewOrderStatusClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"orderdetails?orderId={_orderId}");
    }

    public async void OnContinueShoppingTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//CustomerHome");
    }
}