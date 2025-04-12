using System;
using System.ComponentModel.DataAnnotations;

namespace AnastasiiaPortfolio.Models
{
    public class PlayerScore
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PlayerName { get; set; } = string.Empty;
        
        public int Score { get; set; }
        
        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    }
} 