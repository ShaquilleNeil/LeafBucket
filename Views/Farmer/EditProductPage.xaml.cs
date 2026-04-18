using LeafBucket.Helpers;
using LeafBucket.Models;
using LeafBucket.Services;
using LeafBucket.ViewModels.Farmer;

namespace LeafBucket.Views.Farmer;

public partial class EditProductPage : ContentPage
{

	private Product product = new Product();
    private byte[] _imageData;
    private ProductService productService = new ProductService();
    private StorageService storageService = new StorageService();
    public EditProductPage(Product product)
	{
		this.product = product;
		InitializeComponent();
        LoadProduct();

    }

    private void LoadProduct()
    {
        nameEntry.Text = product.name;
        priceEntry.Text = product.price.ToString();
        stockEntry.Text = product.stockQuantity.ToString();
        productDescription.Text = product.description;

        
        categoryPicker.SelectedItem = product.category;

        
        unitPicker.SelectedItem = product.unit;

       
        if (!string.IsNullOrEmpty(product.imageUrl))
            productImage.Source = ImageSource.FromUri(new Uri(product.imageUrl));
    }

    public async void PickImage_Clicked(object sender, EventArgs e)
    {

        var status = await Permissions.RequestAsync<Permissions.Photos>();
        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert("Permission Denied", "Please allow photo access to upload images.", "OK");
            return;
        }
        var photo = await MediaPicker.PickPhotoAsync();

        if (photo == null)
        {
            return;
        }

        using var stream = await photo.OpenReadAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        _imageData = memoryStream.ToArray();

        productImage.Source = ImageSource.FromFile(photo.FullPath);
    }


    public async void saveProduct(object sender, EventArgs e)
    {
        string name = nameEntry.Text;
        string category = categoryPicker.SelectedItem?.ToString();
        string unit = unitPicker.SelectedItem?.ToString();
        string priceText = priceEntry.Text;
        string quantityText = stockEntry.Text;
        string description = productDescription.Text;

        if (string.IsNullOrEmpty(name))
        {
            await DisplayAlert("Error", "Please enter a product name", "OK");
            return;
        }
        if (string.IsNullOrEmpty(category))
        {
            await DisplayAlert("Error", "Please pick a category", "OK");
            return;
        }
        if (string.IsNullOrEmpty(unit))
        {
            await DisplayAlert("Error", "Please pick a unit", "OK");
            return;
        }

        if (string.IsNullOrEmpty(priceText))
        {

            await DisplayAlert("Error", "Please enter a price", "OK");
            return;
        }

        if (string.IsNullOrEmpty(quantityText))
        {
            await DisplayAlert("Error", "Please enter a quantity", "OK");
            return;
        }

        if (string.IsNullOrEmpty(description))
        {
            await DisplayAlert("Error", "Please enter a product description", "OK");
            return;
        }


        double price = double.Parse(priceText);
        int quantity = int.Parse(quantityText);
  

        try
        {
            string imageUrl;
            if (_imageData != null)
            {
                var fileName = $"products/{Guid.NewGuid()}.jpg";
                imageUrl = await storageService.uploadImage(_imageData, fileName);
            }
            else
            {
                imageUrl = this.product.imageUrl;
            }

            var updatedProduct = new Product();
            updatedProduct.productId = this.product.productId;
            updatedProduct.imageUrl = imageUrl;
            updatedProduct.name = name;
            updatedProduct.category = category;
            updatedProduct.price = price;
            updatedProduct.unit = unit;
            updatedProduct.description = description;
            updatedProduct.stockQuantity = quantity;
            updatedProduct.farmerId = SessionManager.UserId;
            updatedProduct.location = SessionManager.Location;
            updatedProduct.isAvailable = true;

            await productService.updateProduct(updatedProduct);
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to update product: {ex.Message}", "OK");
            return;

        }



    }



}



