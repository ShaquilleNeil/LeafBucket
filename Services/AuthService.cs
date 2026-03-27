using System.Net.Http.Json;
using System.Text.Json;
using LeafBucket.Helpers;
using LeafBucket.Models;

namespace LeafBucket.Services
{
    public class AuthService
    {
        private const string ApiKey = "AIzaSyBAOYgdYOimcRj5s4492INJBfes5dd72Sc";
        private readonly HttpClient _httpClient;
     

        private const string projectId = "leafbucket-ed79a";

        public AuthService()
        {
            _httpClient = new HttpClient();
        
        }


        public async Task<FirebaseAuthResponse> SignUp(string email, string password)
        {
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={ApiKey}";

            var signUpData = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var response = await _httpClient.PostAsJsonAsync(url, signUpData);
            var content = await response.Content.ReadAsStringAsync();

            if(!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error signing up: {content}");
            }

            var result = JsonSerializer.Deserialize<FirebaseAuthResponse>(content);

if (result == null)
    throw new Exception("Failed to parse Firebase response");

return result;
            

        }


        public async Task<FirebaseAuthResponse> SignIn(string email, string password)
        {
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}";

            var signInData = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var response = await _httpClient.PostAsJsonAsync(url, signInData);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error signing in: {content}");
            }

            var result = JsonSerializer.Deserialize<FirebaseAuthResponse>(content);
            SessionManager.UserId = result?.localId;
            SessionManager.IdToken = result?.idToken;

if (result == null)
    throw new Exception("Failed to parse Firebase response");


    Console.WriteLine($"Signing in with API key: {ApiKey}");

return result;
        }


         //save user to document in firestore
       public async Task CreateUser(User user, string userId, string idToken)
{
    var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/users?documentId={userId}";

_httpClient.DefaultRequestHeaders.Clear();
    _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

    var body = new
    {
        fields = new
        {
            firstName = new { stringValue = user.firstName },
            lastName = new { stringValue = user.lastName },
            email = new { stringValue = user.email },
            address = new { stringValue = user.address },
            phoneNumber = new { stringValue = user.phoneNumber },
            role = new { stringValue = user.role }
        }
    };

    var response = await _httpClient.PostAsJsonAsync(url, body);

    if (!response.IsSuccessStatusCode)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        throw new Exception($"Error creating user: {errorContent}");
    }
}

           






public async Task<User?> fetchUser(string userId, string idToken)
{
    var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/users/{userId}";

    var request = new HttpRequestMessage(HttpMethod.Get, url);
    request.Headers.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

    var response = await _httpClient.SendAsync(request);
    var content = await response.Content.ReadAsStringAsync();

    Console.WriteLine("🔥 FIRESTORE RESPONSE:");
    Console.WriteLine(content);

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Fetch error: {content}");
        return null;
    }

    var json = JsonDocument.Parse(content);

    // 🔴 CRITICAL CHECK
    if (!json.RootElement.TryGetProperty("fields", out var fields))
    {
        Console.WriteLine("No fields found.");
        return null;
    }

    // ✅ SAFE extractor
    string SafeGet(string key)
    {
        if (fields.TryGetProperty(key, out var prop))
        {
            if (prop.TryGetProperty("stringValue", out var value))
            {
                return value.GetString() ?? "";
            }
        }
        return "";
    }

    var user = new User
    {
        firstName = SafeGet("firstName"),
        lastName = SafeGet("lastName"),
        email = SafeGet("email"),
        address = SafeGet("address"),
        phoneNumber = SafeGet("phoneNumber"),
        role = SafeGet("role")
    };

    Console.WriteLine($"✅ USER ROLE: {user.role}");

    return user;
}


     }



}

