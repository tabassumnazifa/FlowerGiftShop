namespace FlowerShopAuth.Models
{
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1, 5)]
        public int Score { get; set; }

        public string? Comment { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; }

        public string? UserEmail { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
