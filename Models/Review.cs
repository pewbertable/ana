using System;
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AnastasiiaPortfolio.Models
{
    public class Review
    {
        [BsonId]
        public Guid Id { get; set; }

        // Remove DataAnnotations
        public required string Name { get; set; }

        // Remove DataAnnotations
        public int Rating { get; set; }

        // Remove DataAnnotations
        public required string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid? ProjectId { get; set; } // Change to Guid?

        public Guid? UserId { get; set; } // Change to Guid?

        // Remove DataAnnotations
        public string? Title { get; set; }

        // Remove DataAnnotations
        public string? Pros { get; set; }

        // Remove DataAnnotations
        public string? Cons { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsHidden { get; set; }

        public bool IsVerified { get; set; }

        public bool IsFeatured { get; set; }

        public int HelpfulCount { get; set; }

        public int NotHelpfulCount { get; set; }

        public ReviewSortOption SortOption { get; set; }

        // Remove ForeignKey attribute and comment out navigation property
        // [ForeignKey("ProjectId")]
        // public virtual Project? Project { get; set; }

        // Remove ForeignKey attribute and comment out navigation property
        // [ForeignKey("UserId")]
        // public virtual ApplicationUser? User { get; set; }

        // Comment out navigation property
        // public virtual ICollection<ReviewVote> Votes { get; set; } = new List<ReviewVote>();

        public Review()
        {
            Id = Guid.NewGuid();
        }
    }

    // Keep the enum
    public enum ReviewSortOption
    {
        Newest,
        HighestRated,
        LowestRated,
        MostHelpful,
        MostControversial
    }
} 