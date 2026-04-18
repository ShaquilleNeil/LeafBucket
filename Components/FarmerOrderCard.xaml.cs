using LeafBucket.Models;
using LeafBucket.Services;
using LeafBucket.Views.Farmer;

namespace LeafBucket.Components;

public partial class FarmerOrderCard : ContentView
{
    OrderService  _orderService = new OrderService();


    public FarmerOrderCard()
	{
		InitializeComponent();
      
    }

    public static readonly BindableProperty OrderIdProperty =
       BindableProperty.Create(nameof(OrderId), typeof(string), typeof(FarmerOrderCard), default(string));

    public string OrderId
    {
        get => (string)GetValue(OrderIdProperty);
        set => SetValue(OrderIdProperty, value);
    }

    public static readonly BindableProperty StatusProperty =
       BindableProperty.Create(nameof(Status), typeof(string), typeof(FarmerOrderCard), default(string));

    public string Status
    {
        get => (string)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }


    public static readonly BindableProperty DateProperty =
        BindableProperty.Create(nameof(Date), typeof(string), typeof(FarmerOrderCard), default(string));

    public string Date
    {
        get => (string)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }
    public static readonly BindableProperty OrderProperty =
 BindableProperty.Create(nameof(Order), typeof(Order), typeof(FarmerOrderCard), default(Order));

    public Order? Order
    {
        get => (Order?)GetValue(OrderProperty);
        set => SetValue(OrderProperty, value);
    }
    public static readonly BindableProperty SummaryProperty =
       BindableProperty.Create(nameof(Summary), typeof(string), typeof(FarmerOrderCard), default(string));

    public string Summary
    {
        get => (string)GetValue(SummaryProperty);
        set => SetValue(SummaryProperty, value);
    }

    public static readonly BindableProperty TotalProperty =
         BindableProperty.Create(nameof(Total), typeof(string), typeof(FarmerOrderCard), default(string));

    public string Total
    {
        get => (string)GetValue(TotalProperty);
        set => SetValue(TotalProperty, value);

    }


    private async void OnCardTapped(object sender, TappedEventArgs e)
    {
        if (Order == null) return;

        await Navigation.PushAsync(new FarmerOrderDetailsPage(Order));
    }

    private async void OnAcceptClicked(object sender, EventArgs e)
    {
        if (Order == null) return;
        await _orderService.updateOrderStatus(Order.orderId!, "Preparing");
        MessagingCenter.Send(this, "OrderStatusChanged");
    }

    private async void OnDeclineClicked(object sender, EventArgs e)
    {
        if (Order == null) return;
        await _orderService.updateOrderStatus(Order.orderId!, "Cancelled");
        MessagingCenter.Send(this, "OrderStatusChanged");
    }

    private async void OnMarkReadyClicked(object sender, EventArgs e)
    {
        if (Order == null) return;
        await _orderService.updateOrderStatus(Order.orderId!, "Shipped");
        MessagingCenter.Send(this, "OrderStatusChanged");
    }

    private async void OnMarkCompletedClicked(object sender, EventArgs e)
    {
        if (Order == null) return;
        await _orderService.updateOrderStatus(Order.orderId!, "Completed");
        MessagingCenter.Send(this, "OrderStatusChanged");
    }
}