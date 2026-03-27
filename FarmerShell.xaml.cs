using LeafBucket.Views.Customer;
using LeafBucket.Views.Farmer;

namespace LeafBucket;

public partial class FarmerShell : Shell
{
    public FarmerShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("CustomerHome", typeof(HomePage));
        Routing.RegisterRoute("FarmerHome", typeof(DashboardPage));
    }
}
