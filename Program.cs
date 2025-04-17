using Microsoft.AspNetCore.Identity;
// Remove EF Core using statement
// using Microsoft.EntityFrameworkCore;
using AnastasiiaPortfolio.Data;
using AnastasiiaPortfolio.Models;
using AnastasiiaPortfolio.Services;
using FluentValidation.AspNetCore; // Add FluentValidation using
using MongoDB.Driver; // Add MongoDB Driver using
using AspNetCore.Identity.MongoDbCore.Infrastructure; // Add Identity MongoDbCore using
using AspNetCore.Identity.MongoDbCore.Models; // Add Identity MongoDbCore using

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddFluentValidationAutoValidation(); // Register FluentValidation

// Remove DbContext registration
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
// \toptions.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
// \t\tsqlServerOptionsAction: sqlOptions =>
// \t\t{
// \t\t\tsqlOptions.EnableRetryOnFailure(
// \t\t\t\tmaxRetryCount: 5,
// \t\t\t\tmaxRetryDelay: TimeSpan.FromSeconds(30),
// \t\t\t\terrorNumbersToAdd: null);
// \t\t}));

// Configure MongoDB settings
var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbConfig)).Get<MongoDbConfig>();

// Add null check for essential configuration
if (mongoDbSettings == null || string.IsNullOrEmpty(mongoDbSettings.ConnectionString) || string.IsNullOrEmpty(mongoDbSettings.Name))
{
    throw new InvalidOperationException("MongoDB configuration (MongoDbConfig section with ConnectionString and Name) is missing or incomplete in appsettings.json.");
}

builder.Services.AddSingleton<MongoDbConfig>(mongoDbSettings); // Register config object if needed elsewhere

// Register IMongoClient
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    return new MongoClient(mongoDbSettings.ConnectionString); // ConnectionString checked above
});

// Register IMongoDatabase
builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDbSettings.Name);
});

// Configure Identity to use MongoDbCore
// Note: ApplicationUser needs to inherit from MongoIdentityUser<Guid>
// Note: IdentityRole needs to inherit from MongoIdentityRole<Guid>
// We might need to adjust the key type (Guid) if your ApplicationUser uses string
builder.Services.AddIdentity<ApplicationUser, MongoIdentityRole<Guid>>(options => // Assuming Guid key, adjust if needed
{
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireUppercase = true;
	options.Password.RequireNonAlphanumeric = true;
	options.Password.RequiredLength = 8;
})
.AddMongoDbStores<ApplicationUser, MongoIdentityRole<Guid>, Guid>(mongoDbSettings.ConnectionString, mongoDbSettings.Name) // Use MongoDbStores
.AddDefaultTokenProviders();

// Add Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

var app = builder.Build();

// Seed the database (requires async Main)
// Seeding logic now refactored for MongoDB
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		await SeedData.Initialize(services); // ✅ Make sure Program.cs is async
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while seeding the database.");
	}
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

// Optional: only used if NOT using launchSettings.json
// app.Urls.Add("http://localhost:5009");

app.Run();

// Add a class to hold MongoDB configuration
public class MongoDbConfig
{
    public string? Name { get; set; } // Make nullable
    public string? ConnectionString { get; set; } // Make nullable
}
