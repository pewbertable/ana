using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using AspNetCore.Identity.MongoDbCore.Models;

namespace AnastasiiaPortfolio.Models
{
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsAdmin { get; set; }

        public ApplicationUser() : base()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public ApplicationUser(string userName, string email) : base(userName, email)
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
} 