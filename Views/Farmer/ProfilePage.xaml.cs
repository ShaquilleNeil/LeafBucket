using LeafBucket.Helpers;
using LeafBucket.Services;
using LeafBucket.Views.Auth;

namespace LeafBucket.Views.Farmer;

public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService = new AuthService();
    private readonly StorageService _storageService = new StorageService();
    private readonly UserService _userService = new UserService();
    private byte[] _imageData;
    private bool _isEditing = false;

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
        try
        {
            var photo = await MediaPicker.PickPhotoAsync();
            if (photo == null) return;

            using var stream = await photo.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            _imageData = memoryStream.ToArray();

            
            profilePhoto.Source = ImageSource.FromStream(() => new MemoryStream(_imageData));

           
            var fileName = $"profiles/{SessionManager.UserId}.jpg";
            var imageUrl = await _storageService.uploadImage(_imageData, fileName);

            Console.WriteLine($"UPLOAD URL: {imageUrl}");
            await DisplayAlert("Success", "Profile photo updated!", "OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PHOTO ERROR: {ex.Message}");
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void OnEditProfileClicked(object sender, EventArgs e)
    {
        _isEditing = !_isEditing;

        if (_isEditing)
        {
            viewModeStack.IsVisible = false;
            editModeStack.IsVisible = true;
            changePhotoButton.IsVisible = true;
            editSaveButton.Text = "SAVE CHANGES";
            editSaveButton.BackgroundColor = Color.FromArgb("#2E7D32");
            editSaveButton.TextColor = Colors.White;
            editSaveButton.BorderWidth = 0;

            farmNameEntry.Text = farmNameLabel.Text == "Not set" ? "" : farmNameLabel.Text;
            locationEntry.Text = locationLabel.Text == "Not set" ? "" : locationLabel.Text;
            phoneEntry.Text = phoneLabel.Text.Replace("Ph: ", "");
        }
        else
        {
            _ = SaveProfile();
        }
    }

    private async Task SaveProfile()
    {
        try
        {
            farmNameLabel.Text = farmNameEntry.Text;
            locationLabel.Text = locationEntry.Text;
            phoneLabel.Text = $"Ph: {phoneEntry.Text}";

            viewModeStack.IsVisible = true;
            editModeStack.IsVisible = false;
            changePhotoButton.IsVisible = false;
            editSaveButton.Text = "EDIT PROFILE";
            editSaveButton.BackgroundColor = Colors.White;
            editSaveButton.TextColor = Color.FromArgb("#2E7D32");
            editSaveButton.BorderWidth = 1.5;

            _isEditing = false;

            await DisplayAlert("Success", "Profile updated!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        SessionManager.UserId = null;
        SessionManager.IdToken = null;
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}