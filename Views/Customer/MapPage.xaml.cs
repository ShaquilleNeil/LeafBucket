using LeafBucket.Models;
using LeafBucket.Services;

namespace LeafBucket.Views.Customer;

public partial class MapPage : ContentPage
{
    private readonly UserService _userService = new();
    private List<User> _farmers = new();
    private User? _selectedFarmer;
    private bool _isMapActive = false;

    public MapPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        _isMapActive = true;
        base.OnAppearing();
        _ = LoadMap();
    }

    protected override void OnDisappearing()
    {
        _isMapActive = false;
        base.OnDisappearing();
    }

    private async Task LoadMap()
    {
        try
        {
            loadingOverlay.IsVisible = true;
            farmerCard.IsVisible = false;

            _farmers = await _userService.fetchAllFarmers();

            var pins = new List<object>();
            foreach (var farmer in _farmers)
            {
                if (string.IsNullOrEmpty(farmer.address)) continue;
                if (farmer.latitude == 0 && farmer.longitude == 0) continue;

                var firstName = farmer.firstName ?? "";
                var lastName = farmer.lastName ?? "";
                var initials = $"{(firstName.Length > 0 ? firstName[0] : ' ')}{(lastName.Length > 0 ? lastName[0] : ' ')}".ToUpper().Trim();

                pins.Add(new
                {
                    id = farmer.id,
                    name = !string.IsNullOrEmpty(farmer.farmName) ? farmer.farmName : $"{firstName} {lastName}".Trim(),
                    address = farmer.address,
                    lat = farmer.latitude,
                    lng = farmer.longitude,
                    photo = farmer.profilePhoto ?? "",
                    initials = initials
                });
            }

            var pinsJson = System.Text.Json.JsonSerializer.Serialize(pins);
            var html = BuildMapHtml(pinsJson);

            mapWebView.Source = new HtmlWebViewSource { Html = html };

            mapWebView.Navigated += async (s, e) => { };

            Application.Current?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var result = await mapWebView.EvaluateJavaScriptAsync("window.selectedFarmerId || ''");
                        if (!string.IsNullOrEmpty(result) && result != "null")
                        {
                            var farmerId = result.Trim('"');
                            var farmer = _farmers.FirstOrDefault(f => f.id == farmerId);
                            if (farmer != null && _selectedFarmer?.id != farmerId)
                            {
                                _selectedFarmer = farmer;
                                farmerNameLabel.Text = $"{farmer.firstName} {farmer.lastName}";
                                farmerAddressLabel.Text = farmer.address;
                                farmerCard.IsVisible = true;
                            }
                        }
                    }
                    catch { }
                });
                return _isMapActive;
            }); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MAP ERROR: {ex.Message}");
            Console.WriteLine($"MAP STACK: {ex.StackTrace}");
            await DisplayAlert("Error", $"Could not load map: {ex.Message}", "OK");
        }
        finally
        {
            loadingOverlay.IsVisible = false;
        }
    }

    private string BuildMapHtml(string pinsJson)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        html, body, #map {{ width: 100%; height: 100vh; }}
    </style>
</head>
<body>
    <div id='map'></div>
    <script>
        var farmers = {pinsJson};

        function initMap() {{
            var montreal = {{ lat: 45.5017, lng: -73.5673 }};
            var map = new google.maps.Map(document.getElementById('map'), {{
                zoom: 11,
                center: montreal,
                mapTypeControl: false,
                fullscreenControl: false,
                streetViewControl: false,
                mapId: 'DEMO_MAP_ID'
            }});

            farmers.forEach(function(farmer) {{

                var container = document.createElement('div');
                container.style.cssText = 'display:flex;flex-direction:column;align-items:center;cursor:pointer;';

                var teardrop = document.createElement('div');
                teardrop.style.cssText = `
                    width: 52px;
                    height: 52px;
                    border-radius: 50% 50% 50% 0;
                    transform: rotate(-45deg);
                    background: #2E7D32;
                    border: 3px solid white;
                    box-shadow: 0 2px 8px rgba(0,0,0,0.35);
                    overflow: hidden;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                `;

               
                var img = document.createElement('img');
                img.src = farmer.photo || '';
                img.style.cssText = 'width:100%;height:100%;object-fit:cover;transform:rotate(45deg);';
                img.onerror = function() {{
                    img.style.display = 'none';
                    fallback.style.display = 'flex';
                }};

               
                var fallback = document.createElement('div');
                fallback.style.cssText = `
                    display: ${{farmer.photo ? 'none' : 'flex'}};
                    width: 100%;
                    height: 100%;
                    background: #1B5E20;
                    color: white;
                    font-size: 17px;
                    font-weight: bold;
                    align-items: center;
                    justify-content: center;
                    transform: rotate(45deg);
                    font-family: sans-serif;
                `;
                fallback.textContent = farmer.initials;

                teardrop.appendChild(img);
                teardrop.appendChild(fallback);

                
                var label = document.createElement('div');
                label.style.cssText = `
                    background: #2E7D32;
                    color: white;
                    padding: 3px 8px;
                    border-radius: 10px;
                    font-size: 11px;
                    font-weight: bold;
                    margin-top: 4px;
                    white-space: nowrap;
                    box-shadow: 0 1px 4px rgba(0,0,0,0.2);
                    font-family: sans-serif;
                `;
                label.textContent = farmer.name;

                container.appendChild(teardrop);
                container.appendChild(label);

                var marker = new google.maps.marker.AdvancedMarkerElement({{
                    position: {{ lat: farmer.lat, lng: farmer.lng }},
                    map: map,
                    content: container,
                    title: farmer.farmName
                }});
marker.addListener('click', function() {{
    window.selectedFarmerId = farmer.id;
}});
            }});
        }}
    </script>
    <script async defer
        src='https://maps.googleapis.com/maps/api/js?key=AIzaSyBmMYXLuJOtu1upsQvgIqZwgTzvVfA7lFw&libraries=marker&callback=initMap'>
    </script>
</body>
</html>";
    }

    private async void OnViewProductsClicked(object sender, EventArgs e)
    {
        if (_selectedFarmer == null) return;

        await Shell.Current.GoToAsync("farmerproducts", new Dictionary<string, object>
        {
            { "FarmerId", _selectedFarmer.id }
        });
    }
}