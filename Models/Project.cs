using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnastasiiaPortfolio.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public string Technologies { get; set; } = string.Empty;

        public string? ProjectUrl { get; set; }

        public string? GitHubUrl { get; set; }

        public DateTime DateCompleted { get; set; }

        public bool IsFeatured { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
} 