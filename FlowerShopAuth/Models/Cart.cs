using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowerShopAuth.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!; // Gift Name

        [Required]
        public int Count { get; set; } // Quantity

        [Required]
        public decimal Price { get; set; } // Total price for this item

        // Optional: link to Gift
        [ForeignKey("Gift")]
        public int? GiftId { get; set; }
        public Gift? Gift { get; set; }

        // Optional: link to User
        public string? UserId { get; set; }
    }
}
