
namespace LeafBucket.Views.Customer;

public partial class PaymentMethodPage : ContentPage
{
    private string _selectedPaymentMethod = "CreditCard"; 

    public PaymentMethodPage()
	{
		InitializeComponent();
	}


	public void OnPaymentMethodSelected(object sender, TappedEventArgs e) {
        var args = e as TappedEventArgs;
        string selected = args?.Parameter?.ToString();
        _selectedPaymentMethod = selected ?? "CreditCard";
    }

	public async void OnContinueClicked(object sender, EventArgs e) {

        await Shell.Current.GoToAsync($"OrderSummaryPage?paymentMethod={_selectedPaymentMethod}");
    }

    public void OnPaymentSelected(object sender, TappedEventArgs e)
    {
    }
}