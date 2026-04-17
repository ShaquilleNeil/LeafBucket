using LeafBucket.Views.Customer;
using LeafBucket.Views.Farmer;


namespace LeafBucket;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("CustomerHome", typeof(HomePage));
        Routing.RegisterRoute("FarmerHome", typeof(DashboardPage));
        Routing.RegisterRoute("orderdetails", typeof(OrderDetailsPage));
        Routing.RegisterRoute("OrderSummaryPage", typeof(OrderSummaryPage));
        Routing.RegisterRoute("OrderConfirmationPage", typeof(OrderConfirmationPage));
        Routing.RegisterRoute("addproduct", typeof(AddProductPage));
    }
}
