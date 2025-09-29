using System;
using System.ComponentModel.DataAnnotations;

namespace FlowerShopAuth.Models
{
    public class Offer
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;  // e.g., "Spring Sale"

        [Required]
        [Display(Name = "Minimum Purchase Amount")]
        public decimal MinimumAmount { get; set; }   // If user spends at least this much

        [Required]
        [Display(Name = "Discount (%)")]
        public decimal DiscountPercentage { get; set; }  // e.g., 10 for 10% off

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);
    }
}
