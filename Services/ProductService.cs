using LeafBucket.Helpers;
using LeafBucket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LeafBucket.Services
{
   

    public class ProductService
    {
        private const string ApiKey = "AIzaSyBAOYgdYOimcRj5s4492INJBfes5dd72Sc";
        private readonly HttpClient _httpClient;
        private const string projectId = "leafbucket-ed79a";


        public ProductService() {
            _httpClient = new HttpClient();
        }

        public async Task addProduct(Product product)
        {
            var userId = SessionManager.UserId;
            var idToken = SessionManager.IdToken;

            var generatedId = Guid.NewGuid().ToString();
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/products?documentId={generatedId}&key={ApiKey}";
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);


            var body = new
            {
                fields = new
                {
                    productId = new { stringValue = generatedId },
                    farmerId = new { stringValue = userId },
                    name = new { stringValue = product.name },
                    description = new { stringValue = product.description },
                    category = new { stringValue = product.category },
                    price = new { doubleValue = product.price },
                    unit = new { stringValue = product.unit },
                    stockQuantity = new { integerValue = product.stockQuantity },
                    imageUrl = new { stringValue = product.imageUrl },
                    location = new { stringValue = SessionManager.Location },
                    isAvailable = new { booleanValue = product.isAvailable },
                    createdAt = new { timestampValue = DateTime.UtcNow.ToString("o") },
                    updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, body);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error adding product: {errorContent}");
            }

        }


        public async Task updateProduct(Product product) {
            var idToken = SessionManager.IdToken;
            var location = SessionManager.Location;
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/products/{product.productId}?key={ApiKey}&updateMask.fieldPaths=name&updateMask.fieldPaths=description&updateMask.fieldPaths=category&updateMask.fieldPaths=price&updateMask.fieldPaths=unit&updateMask.fieldPaths=stockQuantity&updateMask.fieldPaths=imageUrl&updateMask.fieldPaths=location&updateMask.fieldPaths=isAvailable&updateMask.fieldPaths=updatedAt";


            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var body = new
            {
                fields = new
                {
                    name = new { stringValue = product.name },
                    description = new { stringValue = product.description },
                    category = new { stringValue = product.category },
                    price = new { doubleValue = product.price },
                    unit = new { stringValue = product.unit },
                    stockQuantity = new { integerValue = product.stockQuantity },
                    imageUrl = new { stringValue = product.imageUrl },
                    location = new { stringValue = location },
                    isAvailable = new { booleanValue = product.isAvailable },
                    updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
                }
            };
            var response = await _httpClient.PatchAsJsonAsync(url, body);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error updating product: {errorContent}");
            }


        }

        public async Task deleteProduct(Product product) {
            var idToken = SessionManager.IdToken;

            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/products";

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{url}/{product.productId}?key={ApiKey}");
            request.Headers.Authorization =
               new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Delete error: {response}");
                throw new Exception($"Error deleting product: {response}");
            }


        }


        public async Task<List<Product>> fetchFarmerProducts() { 
            var userId = SessionManager.UserId;
            var idToken = SessionManager.IdToken;
           


            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents:runQuery";

          

            var body = new
            {
                structuredQuery = new
                {
                    from = new[] { new { collectionId = "products" } },
                    where = new
                    {
                        fieldFilter = new
                        {
                            field = new { fieldPath = "farmerId" },
                            op = "EQUAL",
                            value = new { stringValue = userId }
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync($"{url}?key={ApiKey}", body);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error fetching products: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var products = new List<Product>();

            foreach (var item in json.RootElement.EnumerateArray())
            {
                if (!item.TryGetProperty("document", out var document))
                    continue;

                if (!document.TryGetProperty("fields", out var fields))
                    continue;

                string SafeGet(string key)
                {
                    if (fields.TryGetProperty(key, out var prop))
                    {
                        if (prop.TryGetProperty("stringValue", out var value))
                            return value.GetString() ?? "";
                    }
                    return "";
                }

                var product = new Product
                {
                    productId = document.GetProperty("name").GetString()?.Split('/').Last() ?? "",
                    farmerId = SafeGet("farmerId"),
                    name = SafeGet("name"),
                    description = SafeGet("description"),
                    category = SafeGet("category"),
                    price = fields.TryGetProperty("price", out var price) ? price.GetProperty("doubleValue").GetDouble() : 0,
                    unit = SafeGet("unit"),
                    stockQuantity = fields.TryGetProperty("stockQuantity", out var qty) ? int.Parse(qty.GetProperty("integerValue").GetString() ?? "0") : 0,
                    imageUrl = SafeGet("imageUrl"),
                    location = SafeGet("location"),
                    isAvailable = fields.TryGetProperty("isAvailable", out var avail) && avail.GetProperty("booleanValue").GetBoolean()
                };

                products.Add(product);
            }

            return products;
        }

        public async Task<List<Product>> fetchAllProducts()
        {

            var idToken = SessionManager.IdToken;



            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents:runQuery";


            var body = new
            {
                structuredQuery = new
                {
                    from = new[] { new { collectionId = "products" } },
                    where = new
                    {
                        fieldFilter = new
                        {
                            field = new { fieldPath = "isAvailable" },
                            op = "EQUAL",
                            value = new { booleanValue = true }
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync($"{url}?key={ApiKey}", body);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error fetching products: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var products = new List<Product>();

            foreach (var item in json.RootElement.EnumerateArray())
            {
                if (!item.TryGetProperty("document", out var document))
                    continue;

                if (!document.TryGetProperty("fields", out var fields))
                    continue;

                string SafeGet(string key)
                {
                    if (fields.TryGetProperty(key, out var prop))
                    {
                        if (prop.TryGetProperty("stringValue", out var value))
                            return value.GetString() ?? "";
                    }
                    return "";
                }

                var product = new Product
                {
                    productId = document.GetProperty("name").GetString()?.Split('/').Last() ?? "",
                    farmerId = SafeGet("farmerId"),
                    name = SafeGet("name"),
                    description = SafeGet("description"),
                    category = SafeGet("category"),
                    price = fields.TryGetProperty("price", out var price) ? price.GetProperty("doubleValue").GetDouble() : 0,
                    unit = SafeGet("unit"),
                    stockQuantity = fields.TryGetProperty("stockQuantity", out var qty) ? int.Parse(qty.GetProperty("integerValue").GetString() ?? "0") : 0,
                    imageUrl = SafeGet("imageUrl"),
                    location = SafeGet("location"),
                    isAvailable = fields.TryGetProperty("isAvailable", out var avail) && avail.GetProperty("booleanValue").GetBoolean()
                };

                products.Add(product);
            }

            return products;
        }

        public async Task<List<Product>> fetchProductsByFarmerID(string farmerId)
        {
            //var userId = SessionManager.UserId;
            var idToken = SessionManager.IdToken;



            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents:runQuery";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var body = new
            {
                structuredQuery = new
                {
                    from = new[] { new { collectionId = "products" } },
                    where = new
                    {
                        fieldFilter = new
                        {
                            field = new { fieldPath = "farmerId" },
                            op = "EQUAL",
                            value = new { stringValue = farmerId }
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync($"{url}?key={ApiKey}", body);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error fetching products: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var products = new List<Product>();

            foreach (var item in json.RootElement.EnumerateArray())
            {
                if (!item.TryGetProperty("document", out var document))
                    continue;

                if (!document.TryGetProperty("fields", out var fields))
                    continue;

                string SafeGet(string key)
                {
                    if (fields.TryGetProperty(key, out var prop))
                    {
                        if (prop.TryGetProperty("stringValue", out var value))
                            return value.GetString() ?? "";
                    }
                    return "";
                }

                var product = new Product
                {
                    productId = document.GetProperty("name").GetString()?.Split('/').Last() ?? "",
                    farmerId = SafeGet("farmerId"),
                    name = SafeGet("name"),
                    description = SafeGet("description"),
                    category = SafeGet("category"),
                    price = fields.TryGetProperty("price", out var price) ? price.GetProperty("doubleValue").GetDouble() : 0,
                    unit = SafeGet("unit"),
                    stockQuantity = fields.TryGetProperty("stockQuantity", out var qty) ? int.Parse(qty.GetProperty("integerValue").GetString() ?? "0") : 0,
                    imageUrl = SafeGet("imageUrl"),
                    location = SafeGet("location"),
                    isAvailable = fields.TryGetProperty("isAvailable", out var avail) && avail.GetProperty("booleanValue").GetBoolean()
                };

                products.Add(product);
            }

            return products;
        }








    }
}
