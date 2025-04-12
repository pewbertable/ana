using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnastasiiaPortfolio.Models;
using AnastasiiaPortfolio.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace AnastasiiaPortfolio.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // GET: Review/Create
        [Authorize]
        public async Task<IActionResult> Create(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var review = new Review
            {
                ProjectId = projectId,
                Project = project,
                UserId = user.Id,
                User = user,
                CreatedAt = DateTime.UtcNow,
                Name = user.UserName ?? "Anonymous",
                Comment = string.Empty
            };

            return View(review);
        }

        // POST: Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Review review)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                review.UserId = user.Id;
                review.CreatedAt = DateTime.UtcNow;
                review.IsHidden = false;

                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Projects", new { id = review.ProjectId });
            }
            return View(review);
        }

        // GET: Review/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Project)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || (review.UserId != user.Id && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            return View(review);
        }

        // POST: Review/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,Rating,Title,Comment,Pros,Cons")] Review review)
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

            var user = await _userManager.GetUserAsync(User);
            if (user == null || (existingReview.UserId != user.Id && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingReview.Rating = review.Rating;
                    existingReview.Title = review.Title;
                    existingReview.Comment = review.Comment;
                    existingReview.Pros = review.Pros;
                    existingReview.Cons = review.Cons;
                    existingReview.UpdatedAt = DateTime.UtcNow;

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
                return RedirectToAction("Details", "Projects", new { id = existingReview.ProjectId });
            }
            return View(review);
        }

        // POST: Review/Vote
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Vote(int reviewId, bool isHelpful)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var existingVote = await _context.ReviewVotes
                .FirstOrDefaultAsync(v => v.ReviewId == reviewId && v.UserId == user.Id);

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
                    UserId = user.Id,
                    IsHelpful = isHelpful
                });

                if (isHelpful)
                    review.HelpfulCount++;
                else
                    review.NotHelpfulCount++;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, helpfulCount = review.HelpfulCount, notHelpfulCount = review.NotHelpfulCount });
        }

        // POST: Review/Hide/{id}
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Hide(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            review.IsHidden = !review.IsHidden;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdminIndex));
        }

        // POST: Review/Delete/{id}
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Admin");
        }

        // GET: Review/Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex(string? userId = null, bool showHidden = false)
        {
            var query = _context.Reviews
                .Include(r => r.Project)
                .Include(r => r.User)
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
            return View(reviews);
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }

    public class VoteRequest
    {
        public int ReviewId { get; set; }
        public bool IsHelpful { get; set; }
    }
} 