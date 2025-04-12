using System.Collections.Generic;

namespace AnastasiiaPortfolio.Models;

public class HomeViewModel
{
    public IEnumerable<Project> FeaturedProjects { get; set; } = new List<Project>();
    public ContactViewModel ContactForm { get; set; } = new ContactViewModel();
} 