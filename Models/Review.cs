using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace AnastasiiaPortfolio.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(500)]
        public required string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? ProjectId { get; set; }

        public string? UserId { get; set; }

        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(500)]
        public string? Pros { get; set; }

        [StringLength(500)]
        public string? Cons { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsHidden { get; set; }

        public bool IsVerified { get; set; }

        public bool IsFeatured { get; set; }

        public int HelpfulCount { get; set; }

        public int NotHelpfulCount { get; set; }

        public ReviewSortOption SortOption { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project? Project { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<ReviewVote> Votes { get; set; } = new List<ReviewVote>();
    }

    public enum ReviewSortOption
    {
        Newest,
        HighestRated,
        LowestRated,
        MostHelpful,
        MostControversial
    }
} 