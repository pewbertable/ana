using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnastasiiaPortfolio.Models;
using AnastasiiaPortfolio.Data;
using System.Threading.Tasks;

namespace AnastasiiaPortfolio.Controllers
{
    public class RateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RateController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            if (ModelState.IsValid)
            {
                review.CreatedAt = DateTime.UtcNow;
                _context.Add(review);
                await _context.SaveChangesAsync();

                // Return the new review as a partial view
                return PartialView("_ReviewPartial", review);
            }
            return BadRequest(ModelState);
        }
    }
} 