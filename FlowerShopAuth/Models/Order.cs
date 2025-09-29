using System;

namespace FlowerShopAuth.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }       // The logged-in user
        public decimal TotalAmount { get; set; } // Before discount
        public decimal FinalAmount { get; set; } // After applying offer
        public int? AppliedOfferId { get; set; } // Optional: offer applied
        public DateTime OrderDate { get; set; }
    }
}
