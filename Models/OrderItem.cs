using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeafBucket.Models
{
    public class OrderItem
    {

        public string productId { get; set; }
        public string name { get; set; }
        public string farmerId { get; set; }
        public int quantity { get; set; }
        public double price { get; set; }
    }
}
