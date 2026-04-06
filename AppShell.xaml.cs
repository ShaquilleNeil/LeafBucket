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
    }
}
