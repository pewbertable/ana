using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AnastasiiaPortfolio.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using AnastasiiaPortfolio.Services;
using System.Threading.Tasks;

namespace AnastasiiaPortfolio.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IEmailService _emailService;

    public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment, IEmailService emailService)
    {
        _logger = logger;
        _environment = environment;
        _emailService = emailService;
    }

    public IActionResult Index()
    {
        // Create a view model for the home page
        var viewModel = new HomeViewModel
        {
            FeaturedProjects = new List<Project>
            {
                new Project
                {
                    Id = 1,
                    Title = "E-Commerce Platform",
                    Description = "A full-stack e-commerce solution built with ASP.NET Core MVC",
                    ImageUrl = "/images/projects/ecommerce.jpg",
                    Technologies = "ASP.NET Core MVC, SQL Server, Entity Framework, Bootstrap",
                    ProjectUrl = "https://example.com/ecommerce",
                    GitHubUrl = "https://github.com/pewbertable/CrmTechTitans",
                    DateCompleted = DateTime.Now.AddMonths(-3),
                    IsFeatured = true,
                    Category = "Web Application"
                },
                // Add more featured projects as needed
            }
        };
        return View(viewModel);
    }

    public IActionResult Portfolio()
    {
        // TODO: Replace with actual data from database
        var projects = new List<Project>
        {
            new Project
            {
                Id = 1,
                Title = "E-Commerce Platform",
                Description = "A full-stack e-commerce solution built with ASP.NET Core MVC",
                ImageUrl = "/images/projects/ecommerce.jpg",
                Technologies = "ASP.NET Core MVC, SQL Server, Entity Framework, Bootstrap",
                ProjectUrl = "https://example.com/ecommerce",
                GitHubUrl = "https://github.com/pewbertable/CrmTechTitans",
                DateCompleted = DateTime.Now.AddMonths(-3),
                IsFeatured = true,
                Category = "Web Application"
            },
            // Add more sample projects here
        };

        return View(projects);
    }

    public IActionResult Resume()
    {
        var resumePath = Path.Combine(_environment.WebRootPath, "files", "resume.pdf");
        if (!System.IO.File.Exists(resumePath))
        {
            _logger.LogWarning("Resume file not found at: {Path}", resumePath);
        }
        return View();
    }

    public IActionResult DownloadResume()
    {
        var resumePath = Path.Combine(_environment.WebRootPath, "files", "AnastasiiaResume.pdf");
        if (!System.IO.File.Exists(resumePath))
        {
            _logger.LogWarning("Resume file not found at: {Path}", resumePath);
            return NotFound("Resume file not found.");
        }

        var memory = new MemoryStream();
        using (var stream = new FileStream(resumePath, FileMode.Open))
        {
            stream.CopyTo(memory);
        }
        memory.Position = 0;

        return File(memory, "application/pdf", "Anastasiia_Resume.pdf");
    }

    public IActionResult DownloadReferences()
    {
        var referencesPath = Path.Combine(_environment.WebRootPath, "files", "AnastasiiaReference.pdf");
        if (!System.IO.File.Exists(referencesPath))
        {
            _logger.LogWarning("References file not found at: {Path}", referencesPath);
            return NotFound("References file not found.");
        }

        var memory = new MemoryStream();
        using (var stream = new FileStream(referencesPath, FileMode.Open))
        {
            stream.CopyTo(memory);
        }
        memory.Position = 0;

        return File(memory, "application/pdf", "Anastasiia_References.pdf");
    }

    [HttpGet]
    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(HomeViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _logger.LogInformation("Contact form submitted. Sending email to anastasiiakhrustaleva@gmail.com");
                string recipient = "anastasiiakhrustaleva@gmail.com";
                string subject = $"Portfolio Contact Form Submission from {model.ContactForm.Name}";
                await _emailService.SendContactFormEmailAsync(recipient, subject, model.ContactForm.Name, model.ContactForm.Email, model.ContactForm.Message);
                
                TempData["MessageSent"] = "Thank you for your message! I'll get back to you soon.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send contact form email.");
                ModelState.AddModelError(string.Empty, "Sorry, there was an error sending your message. Please try again later.");
            }
        }
        
        return View("Index", model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
