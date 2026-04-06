using LeafBucket.ViewModels.Farmer;
using LeafBucket.Models;
using System.Threading.Tasks;
using LeafBucket.Helpers;
using LeafBucket.Services;

namespace LeafBucket.Views.Farmer;

public partial class AddProductPage : ContentPage
{
    private byte[] _imageData;
    private ProductService productService = new ProductService();
    private StorageService storageService = new StorageService();

    public AddProductPage()
	{
		InitializeComponent();
       
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

		if (photo == null) {
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

        if (string.IsNullOrEmpty(name)){
            await DisplayAlert("Error", "Please enter a product name", "OK");
            return;
        }
        if (string.IsNullOrEmpty(category)){
            await DisplayAlert("Error", "Please pick a category", "OK");
            return;
        }
        if (string.IsNullOrEmpty(unit))
        {
            await DisplayAlert("Error", "Please pick a unit", "OK");
            return;
        }

        if (string.IsNullOrEmpty(priceText)) {

            await DisplayAlert("Error", "Please enter a price", "OK");
            return;
        }

        if (string.IsNullOrEmpty(quantityText)) {
            await DisplayAlert("Error", "Please enter a quantity", "OK");
            return;
        }

        if (string.IsNullOrEmpty(description)) {
            await DisplayAlert("Error", "Please enter a product description", "OK");
            return;
        }

        if (_imageData == null)
        {
            await DisplayAlert("Error", "Please select an image", "OK");
            return;
        }

        double price = double.Parse(priceText);
        int quantity = int.Parse(quantityText);

        try
        {
            var fileName = $"products/{Guid.NewGuid()}.jpg";
            var imageUrl = await storageService.uploadImage(_imageData, fileName);

            var product = new Product();

            product.imageUrl = imageUrl;
            product.name = name;
            product.category = category;
            product.price = price;
            product.unit = unit;
            product.description = description;
            product.stockQuantity = quantity;
            product.farmerId = SessionManager.UserId;
            product.location = SessionManager.Location;
            product.isAvailable = true;

            await productService.addProduct(product);

            await Navigation.PopAsync();
        }
        catch (Exception ex) {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        
        

    }
}



