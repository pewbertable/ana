using AnastasiiaPortfolio.Models;
using System.Collections.Generic;

namespace AnastasiiaPortfolio.ViewModels
{
    public class ProjectDetailsViewModel
    {
        public Project? Project { get; set; }
        public List<Review> Reviews { get; set; }
        // Add any other data needed for the view

        public ProjectDetailsViewModel()
        {
            Reviews = new List<Review>();
        }
    }
} 