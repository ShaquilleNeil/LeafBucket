using LeafBucket.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LeafBucket.Services
{
    public class StorageService
    {
        private const string ApiKey = "AIzaSyBAOYgdYOimcRj5s4492INJBfes5dd72Sc";
        private readonly HttpClient _httpClient;
        private const string projectId = "leafbucket-ed79a";

        public StorageService()
        {
            _httpClient = new HttpClient();
        }


        public async Task<string> uploadImage(byte[] imageData, string fileName) {
            var idToken = SessionManager.IdToken;

            var encodedFileName = Uri.EscapeDataString(fileName);

            var url = $"https://firebasestorage.googleapis.com/v0/b/leafbucket-ed79a.firebasestorage.app/o?name={encodedFileName}&key={ApiKey}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);


            var content = new ByteArrayContent(imageData);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            Console.WriteLine($"Upload URL: {url}");
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode) {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error adding image: {errorContent}");
                Console.WriteLine($"Response: {errorContent}");
            }

            

            var responseContent = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(responseContent);
            var token = json.RootElement.GetProperty("downloadTokens").GetString();
            return $"https://firebasestorage.googleapis.com/v0/b/leafbucket-ed79a.firebasestorage.app/o/{encodedFileName}?alt=media&token={token}";
        }








    }
}
