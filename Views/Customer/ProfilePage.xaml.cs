using LeafBucket.Helpers;
using LeafBucket.Services;
using LeafBucket.Views.Auth;

namespace LeafBucket.Views.Customer;

public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService = new AuthService();
    private readonly StorageService _storageService = new StorageService();
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
            customerName.Text = $"{user.firstName} {user.lastName}";
            addressEntry.Text = user.address;
            phoneEntry.Text = user.phoneNumber;
            customerEmail.Text = user.email;

            if (!string.IsNullOrEmpty(user.profilePhoto))
                customerPhoto.Source = ImageSource.FromUri(new Uri(user.profilePhoto));
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

            customerPhoto.Source = ImageSource.FromStream(() => new MemoryStream(_imageData));

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

    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        _isEditing = !_isEditing;

        addressEntry.IsReadOnly = !_isEditing;
        phoneEntry.IsReadOnly = !_isEditing;
        changePhotoButton.IsVisible = _isEditing;

        if (_isEditing)
        {
            editSaveButton.Text = "SAVE CHANGES";
            editSaveButton.BackgroundColor = Color.FromArgb("#2E7D32");
            editSaveButton.TextColor = Colors.White;
            editSaveButton.BorderWidth = 0;

            addressEntry.BackgroundColor = Colors.White;
            phoneEntry.BackgroundColor = Colors.White;
        }
        else
        {
            try
            {
                await _authService.updateCustomer(
                    SessionManager.UserId!,
                    addressEntry.Text ?? "",
                    phoneEntry.Text ?? ""
                );

                editSaveButton.Text = "EDIT PROFILE";
                editSaveButton.BackgroundColor = Colors.White;
                editSaveButton.TextColor = Color.FromArgb("#2E7D32");
                editSaveButton.BorderWidth = 1.5;

                addressEntry.BackgroundColor = Colors.Transparent;
                phoneEntry.BackgroundColor = Colors.Transparent;

                _isEditing = false;

                await DisplayAlert("Success", "Profile updated!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    private void Logout_Clicked(object sender, EventArgs e)
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