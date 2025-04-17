using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore; // Remove EF Core
using AnastasiiaPortfolio.Models;
using AspNetCore.Identity.MongoDbCore.Models; // Add Identity MongoDbCore using
using Microsoft.Extensions.DependencyInjection; // For IServiceProvider extension methods

namespace AnastasiiaPortfolio.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            // Remove context usage
            // using (var context = new ApplicationDbContext(
            //     serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            // {
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                // Use RoleManager for MongoIdentityRole<Guid>
                var roleManager = serviceProvider.GetRequiredService<RoleManager<MongoIdentityRole<Guid>>>();

                var adminRoleName = "Admin";

                // Create Admin role if it doesn't exist
                if (!await roleManager.RoleExistsAsync(adminRoleName))
                {
                    // Create MongoIdentityRole<Guid>
                    await roleManager.CreateAsync(new MongoIdentityRole<Guid>(adminRoleName));
                }

                // Create admin user if it doesn't exist
                // Ensure email and password match your requirements
                var adminEmail = "anastasiiakhru@gmail.com";
                var adminPassword = "Password1!"; // Consider using configuration for the password

                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    // ApplicationUser creation is already compatible
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FirstName = "Admin", // Add required fields if necessary
                        LastName = "User"    // Add required fields if necessary
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        // Add user to the Admin role
                        await userManager.AddToRoleAsync(adminUser, adminRoleName);
                    }
                    else
                    {
                        // Log errors if creation failed
                        // Use ILogger<Program> for static context
                        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                        logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            // }
        }
    }
} 