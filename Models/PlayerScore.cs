using System;
// using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AnastasiiaPortfolio.Models
{
    public class PlayerScore
    {
        [BsonId]
        public Guid Id { get; set; }

        // Remove DataAnnotations
        public string PlayerName { get; set; } = string.Empty;

        public int Score { get; set; }

        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;

        public PlayerScore()
        {
            Id = Guid.NewGuid();
        }
    }
} 