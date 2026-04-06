using LeafBucket.Views.Customer;

namespace LeafBucket.Components;

public partial class OrderCard : ContentView
{
    public OrderCard()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty OrderIdProperty =
        BindableProperty.Create(nameof(OrderId), typeof(string), typeof(OrderCard), default(string));

    public string OrderId
    {
        get => (string)GetValue(OrderIdProperty);
        set => SetValue(OrderIdProperty, value);
    }

    public static readonly BindableProperty StatusProperty =
        BindableProperty.Create(nameof(Status), typeof(string), typeof(OrderCard), default(string));

    public string Status
    {
        get => (string)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public static readonly BindableProperty SummaryProperty =
        BindableProperty.Create(nameof(Summary), typeof(string), typeof(OrderCard), default(string));

    public string Summary
    {
        get => (string)GetValue(SummaryProperty);
        set => SetValue(SummaryProperty, value);
    }

    public static readonly BindableProperty DateProperty =
        BindableProperty.Create(nameof(Date), typeof(string), typeof(OrderCard), default(string));

    public string Date
    {
        get => (string)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    private async void OnCardTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("orderdetails");

    }
}