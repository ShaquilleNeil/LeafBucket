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
            farmNameEntry.Text = user.farmName ?? "";
            locationEntry.Text = user.address ?? "";
            phoneEntry.Text = user.phoneNumber ?? "";
            emailEntry.Text = user.email ?? "";

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
            await _authService.updateProfilePhoto(SessionManager.UserId!, imageUrl);

            await DisplayAlert("Success", "Profile photo updated!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void OnEditProfileClicked(object sender, EventArgs e)
    {
        _isEditing = !_isEditing;

        farmNameEntry.IsReadOnly = !_isEditing;
        locationEntry.IsReadOnly = !_isEditing;
        phoneEntry.IsReadOnly = !_isEditing;
        changePhotoButton.IsVisible = _isEditing;

        if (_isEditing)
        {
            editSaveButton.Text = "SAVE CHANGES";
            editSaveButton.BackgroundColor = Color.FromArgb("#2E7D32");
            editSaveButton.TextColor = Colors.White;
            editSaveButton.BorderWidth = 0;

            farmNameEntry.BackgroundColor = Colors.White;
            locationEntry.BackgroundColor = Colors.White;
            phoneEntry.BackgroundColor = Colors.White;
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
            await _authService.updateUser(
                SessionManager.UserId!,
                farmNameEntry.Text ?? "",
                locationEntry.Text ?? "",
                phoneEntry.Text ?? ""
            );

            editSaveButton.Text = "EDIT PROFILE";
            editSaveButton.BackgroundColor = Colors.White;
            editSaveButton.TextColor = Color.FromArgb("#2E7D32");
            editSaveButton.BorderWidth = 1.5;

            farmNameEntry.BackgroundColor = Colors.Transparent;
            locationEntry.BackgroundColor = Colors.Transparent;
            phoneEntry.BackgroundColor = Colors.Transparent;

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
        SecureStorage.Remove("userId");
        SecureStorage.Remove("idToken");
        SecureStorage.Remove("role");
        SecureStorage.Remove("location");
        SecureStorage.Remove("userName");
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}