using System;
// using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AnastasiiaPortfolio.Models
{
    public class Rating
    {
        [BsonId]
        public Guid Id { get; set; }

        // Remove DataAnnotations
        public int RatingValue { get; set; }

        // Remove DataAnnotations
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid? UserId { get; set; } // Change to Guid?
        public string? UserName { get; set; } // Keep UserName if it's useful

        public Rating()
        {
            Id = Guid.NewGuid();
        }
    }
} 