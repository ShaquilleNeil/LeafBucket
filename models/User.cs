namespace LeafBucket.Models;

public class User
{
    public string? id { get; set; }
    public string? firstName { get; set; }
    public string? lastName { get; set; }
    public string? email { get; set; }
    public string? address { get; set; }
    public string? phoneNumber { get; set; }
    public string? role { get; set; }
    public string? farmName { get; set; }
    public string? profilePhoto { get; set; }

    public double latitude { get; set; }
    public double longitude { get; set; }
}