using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AnastasiiaPortfolio.Data;
using AnastasiiaPortfolio.Models;
using System.Security.Claims;

namespace AnastasiiaPortfolio.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // GET: Reviews/Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin(string? userId = null, bool showHidden = false)
        {
            var query = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Project)
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(r => r.UserId == userId);
            }

            if (!showHidden)
            {
                query = query.Where(r => !r.IsHidden);
            }

            var reviews = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
            var users = await _userManager.Users.ToListAsync();

            ViewBag.Users = users;
            ViewBag.SelectedUserId = userId;
            ViewBag.ShowHidden = showHidden;

            return View(reviews);
        }

        // GET: Reviews/Create
        [Authorize]
        public async Task<IActionResult> Create(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }

            var model = new Review
            {
                ProjectId = projectId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID not found."),
                Name = User.Identity?.Name ?? "Anonymous",
                Comment = string.Empty
            };

            return View(model);
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Review review)
        {
            if (ModelState.IsValid)
            {
                review.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID not found.");
                review.CreatedAt = DateTime.UtcNow;
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Projects", new { id = review.ProjectId });
            }
            return View(review);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,Rating,Comment")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            var existingReview = await _context.Reviews.FindAsync(id);
            if (existingReview == null)
            {
                return NotFound();
            }

            // Check if the current user is the review owner
            if (existingReview.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingReview.Rating = review.Rating;
                    existingReview.Comment = review.Comment;
                    existingReview.UpdatedAt = DateTime.UtcNow;
                    _context.Update(existingReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Projects", new { id = review.ProjectId });
            }
            return RedirectToAction("Details", "Projects", new { id = review.ProjectId });
        }

        // POST: Reviews/Hide/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Hide(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            review.IsHidden = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Projects", new { id = review.ProjectId });
        }

        // POST: Reviews/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, int projectId)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int reviewId, bool isHelpful)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                return NotFound();
            }

            var existingVote = await _context.ReviewVotes
                .FirstOrDefaultAsync(v => v.ReviewId == reviewId && v.UserId == userId);

            if (existingVote != null)
            {
                if (existingVote.IsHelpful == isHelpful)
                {
                    _context.ReviewVotes.Remove(existingVote);
                    if (isHelpful)
                        review.HelpfulCount--;
                    else
                        review.NotHelpfulCount--;
                }
                else
                {
                    existingVote.IsHelpful = isHelpful;
                    if (isHelpful)
                    {
                        review.HelpfulCount++;
                        review.NotHelpfulCount--;
                    }
                    else
                    {
                        review.HelpfulCount--;
                        review.NotHelpfulCount++;
                    }
                }
            }
            else
            {
                _context.ReviewVotes.Add(new ReviewVote
                {
                    ReviewId = reviewId,
                    UserId = userId,
                    IsHelpful = isHelpful,
                    CreatedAt = DateTime.UtcNow
                });

                if (isHelpful)
                    review.HelpfulCount++;
                else
                    review.NotHelpfulCount++;
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                helpfulCount = review.HelpfulCount,
                notHelpfulCount = review.NotHelpfulCount
            });
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
} 