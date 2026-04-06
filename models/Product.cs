using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeafBucket.Models
{
    public class Product
    {
        public string productId { get; set; }
        public string farmerId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public double price { get; set; }
        public string unit { get; set; }
        public int stockQuantity { get; set; }
        public string imageUrl { get; set; }

        public string location { get; set; }

        public bool isAvailable { get; set; }

        public DateTime createdAt { get; set; }


        public DateTime updatedAt { get; set; }

        public string formattedPrice => $"${price:F2} / {unit}";
    }
}
