using LeafBucket.Helpers;
using LeafBucket.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace LeafBucket.Services
{
    public class UserService
    {
        private const string ApiKey = "AIzaSyBAOYgdYOimcRj5s4492INJBfes5dd72Sc";
        private readonly HttpClient _httpClient;
        private const string projectId = "leafbucket-ed79a";

        public UserService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<User>> fetchAllFarmers()
        {
            var idToken = SessionManager.IdToken;
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents:runQuery";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var body = new
            {
                structuredQuery = new
                {
                    from = new[] { new { collectionId = "users" } },
                    where = new
                    {
                        fieldFilter = new
                        {
                            field = new { fieldPath = "role" },
                            op = "EQUAL",
                            value = new { stringValue = "Farmer" }
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync($"{url}?key={ApiKey}", body);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error fetching farmers: {error}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var farmers = new List<User>();

            foreach (var item in json.RootElement.EnumerateArray())
            {
                if (item.TryGetProperty("document", out var document))
                {
                    var fields = document.GetProperty("fields");
                    farmers.Add(new User
                    {
                        id = document.GetProperty("name").GetString()?.Split('/').Last() ?? "",
                        firstName = fields.GetProperty("firstName").GetProperty("stringValue").GetString(),
                        lastName = fields.GetProperty("lastName").GetProperty("stringValue").GetString(),
                        address = fields.GetProperty("address").GetProperty("stringValue").GetString(),
                        role = fields.GetProperty("role").GetProperty("stringValue").GetString(),
                        profilePhoto = fields.TryGetProperty("profilephoto", out var photo)
         ? photo.GetProperty("stringValue").GetString() : "",
                        farmName = fields.TryGetProperty("farmName", out var farm)
         ? farm.GetProperty("stringValue").GetString() : ""
                    });
                }
            }

            return farmers;
        }
    }
}