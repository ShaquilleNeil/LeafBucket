using LeafBucket.Helpers;
using LeafBucket.Models;

namespace LeafBucket.Views.Customer;

public partial class CartPage : ContentPage
{
    private const double DeliveryFee = 0.10;
    private const double TaxRate = 0.05;

    public CartPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshCart();
    }

    private void RefreshCart()
    {
        var items = CartManager.GetCartItems();
        CartCollection.ItemsSource = null;
        CartCollection.ItemsSource = items;

        var subtotal = CartManager.GetTotal();
        var delivery = subtotal * DeliveryFee;
        var tax = subtotal * TaxRate;
        var total = subtotal + delivery + tax;

        subtotalLabel.Text = $"Subtotal: ${subtotal:F2}";
        deliveryLabel.Text = $"Delivery Fee: ${delivery:F2}";
        taxLabel.Text = $"Tax: ${tax:F2}";
        totalLabel.Text = $"Total: ${total:F2}";
    }

    private void Increase_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var item = (CartItem)button.BindingContext;
        if (item.quantity < item.stockQuantity)
        {
            CartManager.UpdateQuantity(item.productId, item.quantity + 1);
            RefreshCart();
        }
    }

    private void Decrease_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var item = (CartItem)button.BindingContext;
        if (item.quantity > 1)
        {
            CartManager.UpdateQuantity(item.productId, item.quantity - 1);
            RefreshCart();
        }
    }

    private void Remove_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var item = (CartItem)button.BindingContext;
        CartManager.RemoveItem(item.productId);
        RefreshCart();
    }

    private async void Checkout_Clicked(object sender, EventArgs e)
    {
        if (!CartManager.GetCartItems().Any())
        {
            await DisplayAlert("Empty Cart", "Please add items before checking out.", "OK");
            return;
        }
        await Navigation.PushAsync(new PaymentMethodPage());
    }
}