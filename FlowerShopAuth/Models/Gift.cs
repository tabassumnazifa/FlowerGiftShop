using System.ComponentModel.DataAnnotations;

namespace FlowerShopAuth.Models
{
    public class Gift
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty; // URL or file path
    }
}
