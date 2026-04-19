using LeafBucket.Helpers;
using LeafBucket.Models;
using LeafBucket.Services;

namespace LeafBucket.Views.Auth;

public partial class SignupPage : ContentPage
{
    private AuthService authService = new AuthService();
    private UserService _userService = new UserService();
    private StorageService _storageService = new StorageService();
    private byte[] _imageData;

    public SignupPage()
    {
        InitializeComponent();
    }

    private void LoginNavigation(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void PickPhoto_Clicked(object sender, EventArgs e)
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
    }

    private void OnRoleChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender == CustomerCheckBox && CustomerCheckBox.IsChecked)
            FarmerCheckBox.IsChecked = false;
        else if (sender == FarmerCheckBox && FarmerCheckBox.IsChecked)
            CustomerCheckBox.IsChecked = false;

        farmNameStack.IsVisible = FarmerCheckBox.IsChecked;
    }

    private async void SignUpButton_Clicked(object sender, EventArgs e)
    {
        FirstNameErrorLabel.IsVisible = false;
        LastNameErrorLabel.IsVisible = false;
        EmailErrorLabel.IsVisible = false;
        PhoneNumberErrorLabel.IsVisible = false;
        AddressErrorLabel.IsVisible = false;
        PasswordErrorLabel.IsVisible = false;
        ConfirmPasswordErrorLabel.IsVisible = false;
        RoleErrorLabel.IsVisible = false;

        string firstName = FirstName.Text ?? "";
        string lastName = LastName.Text ?? "";
        string phoneNumber = PhoneNumber.Text ?? "";
        string address = Address.Text ?? "";
        string email = Email.Text ?? "";
        string password = Password.Text ?? "";
        string confirmPassword = ConfirmPassword.Text ?? "";
        string farmName = FarmName.Text ?? "";
        string role = "";

        if (string.IsNullOrWhiteSpace(firstName))
        {
            FirstNameErrorLabel.Text = "Please enter your first name.";
            FirstNameErrorLabel.IsVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            LastNameErrorLabel.Text = "Please enter your last name.";
            LastNameErrorLabel.IsVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            EmailErrorLabel.Text = "Please enter your email.";
            EmailErrorLabel.IsVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            PhoneNumberErrorLabel.Text = "Please enter your phone number.";
            PhoneNumberErrorLabel.IsVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            AddressErrorLabel.Text = "Please enter your address.";
            AddressErrorLabel.IsVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            PasswordErrorLabel.Text = "Please enter your password.";
            PasswordErrorLabel.IsVisible = true;
            return;
        }

        if (password.Length < 6)
        {
            PasswordErrorLabel.Text = "Password must be at least 6 characters.";
            PasswordErrorLabel.IsVisible = true;
            return;
        }

        if (password != confirmPassword)
        {
            ConfirmPasswordErrorLabel.Text = "Passwords do not match.";
            ConfirmPasswordErrorLabel.IsVisible = true;
            return;
        }

        if (!CustomerCheckBox.IsChecked && !FarmerCheckBox.IsChecked)
        {
            RoleErrorLabel.Text = "Please select a role.";
            RoleErrorLabel.IsVisible = true;
            return;
        }

        if (FarmerCheckBox.IsChecked && string.IsNullOrWhiteSpace(farmName))
        {
            FarmNameErrorLabel.Text = "Please enter your farm name.";
            FarmNameErrorLabel.IsVisible = true;
            return;
        }

        role = CustomerCheckBox.IsChecked ? "Customer" : "Farmer";

        var user = new User
        {
            firstName = firstName,
            lastName = lastName,
            phoneNumber = phoneNumber,
            address = address,
            email = email,
            role = role,
            farmName = farmName
        };

        try
        {
            var auth = await authService.SignUp(email, password);

            if (auth == null || string.IsNullOrEmpty(auth.localId))
            {
                await DisplayAlert("Error", "Failed to create account.", "OK");
                return;
            }

            SessionManager.UserId = auth.localId;
            SessionManager.IdToken = auth.idToken;

            if (_imageData != null)
            {
                var fileName = $"profiles/{auth.localId}.jpg";
                user.profilePhoto = await _storageService.uploadImage(_imageData, fileName);
            }

            await authService.CreateUser(user, auth.localId, auth.idToken);


            try
            {
                var coords = await _userService.GeocodeAddress(address);
                if (coords != null)
                {
                    await _userService.saveUserLocation(auth.localId, coords.Value.lat, coords.Value.lng);
                }
            }
            catch
            {
                // skip silently
            }

            await DisplayAlert("Success", "Account created successfully!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Signup Error", ex.Message, "OK");
        }
    }
}