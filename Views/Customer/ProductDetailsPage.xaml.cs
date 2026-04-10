using LeafBucket.ViewModels.Customer;
using LeafBucket.Models;
using LeafBucket.Services;
using LeafBucket.Helpers;

namespace LeafBucket.Views.Customer;

public partial class ProductDetailsPage : ContentPage
{
    private Product _product;
    private int _quantity = 1;
    public ProductDetailsPage(Product product)
	{
		InitializeComponent();
		_product = product;

		productImage.Source = product.imageUrl;
        productName.Text = product.name;
        productPrice.Text = $"${product.price:F2}";
        productDescription.Text = product.description;


    }

    private void Increase_Clicked(object sender, EventArgs e) {
        if (_quantity < _product.stockQuantity) {  
            _quantity++;
            quantityLabel.Text = _quantity.ToString();
        }
    }

    private void Decrease_Clicked(object sender, EventArgs e)
    {
        if (_quantity > 1)
        {
            _quantity--;
            quantityLabel.Text = _quantity.ToString();
        }
    }

    private async void AddToCart_Clicked(object sender, EventArgs e)
    {
        var cartItem = new CartItem
        {
            productId = _product.productId,
            name = _product.name,
            price = _product.price,
            quantity = _quantity,
            imageUrl = _product.imageUrl,
            unit = _product.unit,
            stockQuantity = _product.stockQuantity,
            farmerId = _product.farmerId
        };
        CartManager.AddItem(cartItem);
        await DisplayAlert("Success", "Product added to cart!", "OK");
        await Navigation.PopAsync();
    }


}


