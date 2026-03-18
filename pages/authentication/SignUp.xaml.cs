using LeafBucket.Services;
using LeafBucket.Services.Auth;


namespace LeafBucket.pages.authentication;

public partial class SignUp : ContentPage
{
	private AuthService authService = new AuthService();

	public SignUp()
	{
		InitializeComponent();
	}

	private void LoginNavigation(object sender, EventArgs e)
	{
		Navigation.PushAsync(new Login());
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

    role = CustomerCheckBox.IsChecked ? "Customer" : "Farmer";

    var user = new User
    {
        firstName = firstName,
        lastName = lastName,
        phoneNumber = phoneNumber,
        address = address,
        email = email,
        role = role
    };

    try
    {
        Console.WriteLine("Starting signup...");

        var auth = await authService.SignUp(email, password);
        Console.WriteLine($"Signup success. localId = {auth?.localId}");

        if (auth == null || string.IsNullOrEmpty(auth.localId))
        {
            await DisplayAlert("Error", "Failed to create account.", "OK");
            return;
        }

        await authService.CreateUser(user, auth.localId, auth.idToken);
        Console.WriteLine("Firestore user created.");

        await DisplayAlert("Success", "Account created successfully!", "OK");
        await Navigation.PushAsync(new Login());
    }
    catch (Exception ex)
    {
        Console.WriteLine($"SIGNUP ERROR: {ex}");
        await DisplayAlert("Signup Error", ex.Message, "OK");
    }
}

}