using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeafBucket.Models
{
    public class Review
    {
        public string reviewId { get; set; }

        public string customerId { get; set; }

        public string farmerId { get; set; }
        public string productId { get; set; }
        public string orderId { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
        public List<string> photos {  get; set; }
        public string farmerReply {  get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

    }
}
