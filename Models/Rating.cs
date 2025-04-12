using System;
using System.ComponentModel.DataAnnotations;

namespace AnastasiiaPortfolio.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 5)]
        public int RatingValue { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? UserId { get; set; }
        public string? UserName { get; set; }
    }
} 