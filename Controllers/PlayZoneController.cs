using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnastasiiaPortfolio.Models;
using AnastasiiaPortfolio.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AnastasiiaPortfolio.Controllers
{
    public class PlayZoneController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlayZoneController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var topScores = await _context.PlayerScores
                .OrderByDescending(s => s.Score)
                .Take(3)
                .ToListAsync();
            
            ViewBag.TopScores = topScores;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveScore([FromBody] PlayerScore score)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false });
            }

            score.PlayedAt = DateTime.UtcNow;
            _context.PlayerScores.Add(score);
            await _context.SaveChangesAsync();

            var topScores = await _context.PlayerScores
                .OrderByDescending(s => s.Score)
                .Take(3)
                .ToListAsync();

            return Json(new { success = true, topScores });
        }
    }
} 