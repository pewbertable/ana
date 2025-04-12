using System.ComponentModel.DataAnnotations;

namespace AnastasiiaPortfolio.Models
{
    public class RatingViewModel
    {
        [Required(ErrorMessage = "Please select a rating")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Please provide your feedback")]
        [StringLength(500, ErrorMessage = "Feedback must be less than 500 characters")]
        public string Comment { get; set; } = string.Empty;
    }
} 