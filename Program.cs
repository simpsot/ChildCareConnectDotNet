using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Services;
using ChildCareConnect.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure database
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("DATABASE_URL environment variable is not set.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<ProviderService>();
builder.Services.AddScoped<FormFieldService>();
builder.Services.AddScoped<StatsService>();

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // Check if we can connect to the database
        await dbContext.Database.CanConnectAsync();
        Console.WriteLine("Successfully connected to database");
        
        // Seed initial data if needed
        if (!dbContext.Users.Any())
        {
            await SeedDataAsync(dbContext);
            Console.WriteLine("Database seeded successfully");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection error: {ex.Message}");
        Console.WriteLine("The application will start but database features may not work until the schema is created.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static async Task SeedDataAsync(AppDbContext dbContext)
{
    // Add sample users
    var users = new[]
    {
        new ChildCareConnect.Models.User { Name = "Sarah Johnson", Email = "sarah@example.com", Role = "Admin", Team = "Administration", Status = "Active", Avatar = "SJ" },
        new ChildCareConnect.Models.User { Name = "Michael Chen", Email = "michael@example.com", Role = "Manager", Team = "Family Services", Status = "Active", Avatar = "MC" },
        new ChildCareConnect.Models.User { Name = "Emily Williams", Email = "emily@example.com", Role = "Case Manager", Team = "Family Services", Status = "Active", Avatar = "EW" },
        new ChildCareConnect.Models.User { Name = "David Martinez", Email = "david@example.com", Role = "Coordinator", Team = "Provider Relations", Status = "Active", Avatar = "DM" }
    };
    dbContext.Users.AddRange(users);
    await dbContext.SaveChangesAsync();

    // Add sample clients
    var clients = new[]
    {
        new ChildCareConnect.Models.Client { Name = "The Thompson Family", Contact = "Emily Thompson", Children = 2, Status = "Active", CaseManagerId = users[2].Id, LastContact = "Today" },
        new ChildCareConnect.Models.Client { Name = "The Garcia Family", Contact = "Maria Garcia", Children = 1, Status = "Pending", CaseManagerId = users[2].Id, LastContact = "Yesterday" },
        new ChildCareConnect.Models.Client { Name = "The Wilson Family", Contact = "James Wilson", Children = 3, Status = "Active", CaseManagerId = users[1].Id, LastContact = "2 days ago" }
    };
    dbContext.Clients.AddRange(clients);

    // Add sample providers
    var providers = new[]
    {
        new ChildCareConnect.Models.Provider { Name = "Sunshine Learning Center", Type = "Center", Capacity = 50, Enrollment = 42, Rating = "4.8", Status = "Verified", Location = "Downtown" },
        new ChildCareConnect.Models.Provider { Name = "Little Stars Home Care", Type = "In-Home", Capacity = 6, Enrollment = 5, Rating = "4.9", Status = "Verified", Location = "Eastside" },
        new ChildCareConnect.Models.Provider { Name = "Bright Futures Preschool", Type = "Preschool", Capacity = 30, Enrollment = 28, Rating = "4.7", Status = "Verified", Location = "Westside" }
    };
    dbContext.Providers.AddRange(providers);

    await dbContext.SaveChangesAsync();
}
