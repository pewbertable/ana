using System;
using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AnastasiiaPortfolio.Models
{
    public class Project
    {
        [BsonId]
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string Technologies { get; set; } = string.Empty;

        public string? ProjectUrl { get; set; }

        public string? GitHubUrl { get; set; }

        public DateTime DateCompleted { get; set; }

        public bool IsFeatured { get; set; }

        public string Category { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }

        public Project()
        {
            Id = Guid.NewGuid();
        }
    }
} 