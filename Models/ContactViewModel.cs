using System.ComponentModel.DataAnnotations;

namespace AnastasiiaPortfolio.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Please enter your name")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Please enter your message")]
        public required string Message { get; set; }
    }
} 