using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore; // Remove EF Core
using AnastasiiaPortfolio.Models;
// using AnastasiiaPortfolio.Data; // Remove EF Core Data
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver; // Add MongoDB
using MongoDB.Bson; // Add Bson

namespace AnastasiiaPortfolio.Controllers
{
    public class PlayZoneController : Controller
    {
        // Replace DbContext with IMongoDatabase
        private readonly IMongoCollection<PlayerScore> _playerScoresCollection;

        public PlayZoneController(IMongoDatabase database)
        {
            _playerScoresCollection = database.GetCollection<PlayerScore>("PlayerScores");
        }

        public async Task<IActionResult> Index()
        {
            // Find top 3 scores
            var topScores = await _playerScoresCollection.Find(_ => true)
                .Sort(Builders<PlayerScore>.Sort.Descending(s => s.Score))
                .Limit(3)
                .ToListAsync();

            ViewBag.TopScores = topScores;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveScore([FromBody] PlayerScore score)
        {
            // Re-enable validation check
            if (!ModelState.IsValid)
            {
                // Return validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return Json(new { success = false, errors });
            }

            score.Id = Guid.NewGuid(); // Ensure Id is generated
            score.PlayedAt = DateTime.UtcNow;

            await _playerScoresCollection.InsertOneAsync(score);

            // Fetch updated top scores
            var topScores = await _playerScoresCollection.Find(_ => true)
                .Sort(Builders<PlayerScore>.Sort.Descending(s => s.Score))
                .Limit(3)
                .ToListAsync();

            return Json(new { success = true, topScores });
        }
    }
} 