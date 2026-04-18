using LeafBucket.Helpers;
using LeafBucket.Services;
using LeafBucket.Views.Auth;

namespace LeafBucket.Views.Farmer;

public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService = new AuthService();
    private readonly StorageService _storageService = new StorageService();
    private byte[] _imageData;

    public ProfilePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var user = await _authService.fetchUser(
            SessionManager.UserId,
            SessionManager.IdToken
        );

        if (user != null)
        {
            farmerNameLabel.Text = $"{user.firstName} {user.lastName}";
            farmNameLabel.Text = user.farmName ?? "Not set";
            locationLabel.Text = user.address ?? "Not set";
            phoneLabel.Text = $"Ph: {user.phoneNumber}";
            emailLabel.Text = $"Email: {user.email}";

            if (!string.IsNullOrEmpty(user.profilePhoto))
                profilePhoto.Source = ImageSource.FromUri(new Uri(user.profilePhoto));
            else
                profilePhoto.Source = "farmer_placeholder.png";
        }
    }

    private async void OnChangePhotoClicked(object sender, EventArgs e)
    {
        var status = await Permissions.RequestAsync<Permissions.Photos>();
        if (status != PermissionStatus.Granted) return;

        var photo = await MediaPicker.PickPhotoAsync();
        if (photo == null) return;

        using var stream = await photo.OpenReadAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        _imageData = memoryStream.ToArray();

        profilePhoto.Source = ImageSource.FromFile(photo.FullPath);

      
        try
        {
            var fileName = $"profiles/{SessionManager.UserId}.jpg";
            var imageUrl = await _storageService.uploadImage(_imageData, fileName);
            await DisplayAlert("Success", "Profile photo updated!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Coming Soon", "Edit profile will be available soon.", "OK");
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        SessionManager.UserId = null;
        SessionManager.IdToken = null;
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}