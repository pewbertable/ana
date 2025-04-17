using System;
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AnastasiiaPortfolio.Models
{
    public class ReviewVote
    {
        [BsonId]
        public Guid Id { get; set; }

        // Remove DataAnnotations
        public Guid ReviewId { get; set; } // Change to Guid

        // Remove DataAnnotations
        public Guid UserId { get; set; } // Change to Guid

        // Remove DataAnnotations
        public bool IsHelpful { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Remove ForeignKey attribute and comment out navigation property
        // [ForeignKey("ReviewId")]
        // public virtual Review Review { get; set; } = null!;

        // Remove ForeignKey attribute and comment out navigation property
        // [ForeignKey("UserId")]
        // public virtual ApplicationUser User { get; set; } = null!;

        public ReviewVote()
        {
            Id = Guid.NewGuid();
        }
    }
} 