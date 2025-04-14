using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnastasiiaPortfolio.Models;

public class HomeViewModel
{
    public List<Project> FeaturedProjects { get; set; } = new List<Project>();
    public ContactForm ContactForm { get; set; } = new ContactForm();
}

public class ContactForm
{
    [Required(ErrorMessage = "Please enter your name")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Please enter your email")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Please enter your message")]
    public string Message { get; set; }
} 