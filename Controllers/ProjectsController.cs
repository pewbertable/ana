using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore; // Remove EF Core
// using AnastasiiaPortfolio.Data; // Remove EF Core Data context
using AnastasiiaPortfolio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Add Identity
using System.Security.Claims;
using MongoDB.Driver; // Add MongoDB Driver
using MongoDB.Bson; // Add Bson
using System.Linq.Expressions; // For filters
using AnastasiiaPortfolio.ViewModels; // Add ViewModels using

namespace AnastasiiaPortfolio.Controllers
{
    public class ProjectsController : Controller
    {
        // Replace DbContext with IMongoDatabase and add UserManager
        private readonly IMongoCollection<Project> _projectsCollection;
        private readonly IMongoCollection<Review> _reviewsCollection; // Added for fetching reviews later
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(IMongoDatabase database, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            // Get the collection (collection name often plural of model name)
            _projectsCollection = database.GetCollection<Project>("Projects");
            _reviewsCollection = database.GetCollection<Review>("Reviews"); // Initialize reviews collection
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            // Find all projects
            var projects = await _projectsCollection.Find(_ => true).ToListAsync();

            // TODO: Decide how to handle review filtering/loading if needed in Index view
            // Original EF code filtered out projects with only hidden reviews.
            // This requires a separate query or aggregation in MongoDB.
            // For now, just returning all projects.
            return View(projects);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(Guid? id) // Change id to Guid?
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find project by Id
            var project = await _projectsCollection.Find(p => p.Id == id.Value).FirstOrDefaultAsync();

            if (project == null)
            {
                return NotFound();
            }

            // Fetch associated reviews (only non-hidden)
            var reviews = await _reviewsCollection
                                    .Find(r => r.ProjectId == project.Id && r.IsHidden == false)
                                    .Sort(Builders<Review>.Sort.Descending(r => r.CreatedAt)) // Optional sort
                                    .ToListAsync();

            // Create ViewModel
            var viewModel = new ProjectDetailsViewModel
            {
                Project = project,
                Reviews = reviews
            };

            return View(viewModel); // Pass ViewModel to the view
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // UserId is now Guid, set directly in POST
            return View(new Project());
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Title,Description,ImageUrl,Technologies,ProjectUrl,GitHubUrl,DateCompleted,IsFeatured,Category")] Project project) // Bind relevant fields, Id is generated
        {
            // Re-enable ModelState check
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge(); // Or handle appropriately
                }

                project.UserId = user.Id; // Assign Guid UserId
                project.CreatedAt = DateTime.UtcNow;
                // project.Id = Guid.NewGuid(); // Id should be generated by constructor or MongoDB

                await _projectsCollection.InsertOneAsync(project);
                return RedirectToAction(nameof(Index));
            }
            // If validation fails, return the view with the model to display errors
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id) // Change id to Guid?
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find project by Id
            var project = await _projectsCollection.Find(p => p.Id == id.Value).FirstOrDefaultAsync();
            if (project == null)
            {
                return NotFound();
            }
            // Optional: Check if user is authorized to edit this specific project if needed
            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        // Update Bind attribute - include Id for matching, exclude UserId/CreatedAt
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,ImageUrl,Technologies,ProjectUrl,GitHubUrl,DateCompleted,IsFeatured,Category")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            // Re-enable ModelState check
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }

                // Retrieve existing project to preserve UserId and CreatedAt
                var existingProject = await _projectsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
                if (existingProject == null)
                {
                    return NotFound();
                }

                // Optional: Check ownership/authorization if needed
                // if (existingProject.UserId != user.Id && !User.IsInRole("Admin")) { return Forbid(); }

                // Preserve original UserId and CreatedAt
                project.UserId = existingProject.UserId;
                project.CreatedAt = existingProject.CreatedAt;

                // Replace the existing document
                var result = await _projectsCollection.ReplaceOneAsync(p => p.Id == id, project);

                // Note: ReplaceOneAsync might not throw on concurrency issues unless configured.
                // Checking ModifiedCount can be useful.
                if (!result.IsAcknowledged || result.ModifiedCount == 0)
                {
                     if (!await ProjectExists(project.Id))
                     {
                         return NotFound();
                     }
                     else
                     {
                         // Add a model error if the update didn't happen for some reason
                         ModelState.AddModelError("", "Unable to save changes. The project might have been modified by another user.");
                         return View(project);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // If validation fails, return the view with the model
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id) // Change id to Guid?
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find project by Id
            var project = await _projectsCollection.Find(p => p.Id == id.Value).FirstOrDefaultAsync();
            if (project == null)
            {
                return NotFound();
            }
            // Optional: Check authorization
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id) // Change id to Guid
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // Find the project to check ownership before deleting
            var project = await _projectsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (project == null)
            {
                return NotFound(); // Already deleted or never existed
            }

            // Check ownership (ensure UserId matches)
            if (project.UserId != user.Id)
            {
                return Forbid(); // Or handle as appropriate
            }

            var result = await _projectsCollection.DeleteOneAsync(p => p.Id == id);

            // Optional: Check result.DeletedCount

            // TODO: Decide if related reviews should be deleted as well.
            // If so: await _reviewsCollection.DeleteManyAsync(r => r.ProjectId == id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProjectExists(Guid id) // Change id to Guid
        {
            // Check count of documents matching the Id
            return await _projectsCollection.CountDocumentsAsync(p => p.Id == id) > 0;
        }
    }
} 