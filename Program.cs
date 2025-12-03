using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Services;
using ChildCareConnect.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure database
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(databaseUrl))
{
    throw new InvalidOperationException("DATABASE_URL environment variable is not set.");
}

// Convert DATABASE_URL format to Npgsql connection string format
// From: postgresql://user:password@host:port/database?sslmode=require
// To: Host=host;Port=port;Database=database;Username=user;Password=password;SSL Mode=Require
string connectionString;
try
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var host = uri.Host;
    var port = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    var username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "";
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
    
    connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    Console.WriteLine($"Connecting to database: {host}:{port}/{database}");
}
catch (Exception ex)
{
    throw new InvalidOperationException($"Invalid DATABASE_URL format: {ex.Message}");
}

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Also register DbContext for backwards compatibility
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString), ServiceLifetime.Transient);

// Register services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<ProviderService>();
builder.Services.AddScoped<FormFieldService>();
builder.Services.AddScoped<StatsService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<RelationshipService>();
builder.Services.AddScoped<HouseholdMemberService>();

// Configure Kestrel for Replit environment
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

// Configure forwarded headers for proxy support
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // Ensure database and tables are created
        await dbContext.Database.EnsureCreatedAsync();
        Console.WriteLine("Database schema ensured");
        
        // Seed initial data if needed
        if (!dbContext.Users.Any())
        {
            await SeedDataAsync(dbContext);
            Console.WriteLine("Database seeded successfully");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
        Console.WriteLine("The application will start but database features may not work.");
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

    // Add relationship types if they don't exist
    if (!dbContext.Relationships.Any())
    {
        var relationships = new[]
        {
            new ChildCareConnect.Models.Relationship { Name = "Self", DisplayOrder = 0 },
            new ChildCareConnect.Models.Relationship { Name = "Spouse/Partner", DisplayOrder = 1 },
            new ChildCareConnect.Models.Relationship { Name = "Child", DisplayOrder = 2 },
            new ChildCareConnect.Models.Relationship { Name = "Parent", DisplayOrder = 3 },
            new ChildCareConnect.Models.Relationship { Name = "Sibling", DisplayOrder = 4 },
            new ChildCareConnect.Models.Relationship { Name = "Grandparent", DisplayOrder = 5 },
            new ChildCareConnect.Models.Relationship { Name = "Grandchild", DisplayOrder = 6 },
            new ChildCareConnect.Models.Relationship { Name = "Other Relative", DisplayOrder = 7 },
            new ChildCareConnect.Models.Relationship { Name = "Non-Relative", DisplayOrder = 8 }
        };
        dbContext.Relationships.AddRange(relationships);
        await dbContext.SaveChangesAsync();
    }

    // Add sample clients
    var clients = new[]
    {
        new ChildCareConnect.Models.Client { Name = "The Thompson Family", Contact = "Emily Thompson", HouseholdSize = 4, Status = "Active", CaseManagerId = users[2].Id, LastContact = "Today" },
        new ChildCareConnect.Models.Client { Name = "The Garcia Family", Contact = "Maria Garcia", HouseholdSize = 3, Status = "Pending", CaseManagerId = users[2].Id, LastContact = "Yesterday" },
        new ChildCareConnect.Models.Client { Name = "The Wilson Family", Contact = "James Wilson", HouseholdSize = 5, Status = "Active", CaseManagerId = users[1].Id, LastContact = "2 days ago" }
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

    // Add sample tags
    var tags = new[]
    {
        new ChildCareConnect.Models.Tag { Name = "Urgent", Color = "red", CreatedById = users[0].Id },
        new ChildCareConnect.Models.Tag { Name = "Follow-up", Color = "orange", CreatedById = users[0].Id },
        new ChildCareConnect.Models.Tag { Name = "Enrollment", Color = "blue", CreatedById = users[0].Id },
        new ChildCareConnect.Models.Tag { Name = "Documentation", Color = "purple", CreatedById = users[0].Id },
        new ChildCareConnect.Models.Tag { Name = "Review", Color = "teal", CreatedById = users[0].Id }
    };
    dbContext.Tags.AddRange(tags);
    await dbContext.SaveChangesAsync();

    // Add sample tasks
    var tasks = new[]
    {
        new ChildCareConnect.Models.TaskItem { Title = "Review Thompson Family application", Description = "Complete the annual review for the Thompson family", Priority = "High", Status = "Pending", AssigneeId = users[0].Id, CreatorId = users[1].Id, DueDate = DateTime.UtcNow.AddDays(3) },
        new ChildCareConnect.Models.TaskItem { Title = "Schedule provider site visit", Description = "Arrange site visit for Sunshine Learning Center", Priority = "Normal", Status = "In Progress", AssigneeId = users[0].Id, CreatorId = users[0].Id, DueDate = DateTime.UtcNow.AddDays(7) },
        new ChildCareConnect.Models.TaskItem { Title = "Update Garcia Family documentation", Description = "Complete pending paperwork for the Garcia family", Priority = "Urgent", Status = "Pending", AssigneeId = users[2].Id, CreatorId = users[1].Id, DueDate = DateTime.UtcNow.AddDays(1) },
        new ChildCareConnect.Models.TaskItem { Title = "Process new provider application", Description = "Review and process application for new childcare provider", Priority = "Normal", Status = "Pending", AssigneeId = users[1].Id, CreatorId = users[0].Id, DueDate = DateTime.UtcNow.AddDays(5) }
    };
    dbContext.Tasks.AddRange(tasks);

    // Add task tags
    dbContext.TaskTags.Add(new ChildCareConnect.Models.TaskTag { TaskId = tasks[0].Id, TagId = tags[4].Id });
    dbContext.TaskTags.Add(new ChildCareConnect.Models.TaskTag { TaskId = tasks[2].Id, TagId = tags[0].Id });
    dbContext.TaskTags.Add(new ChildCareConnect.Models.TaskTag { TaskId = tasks[2].Id, TagId = tags[3].Id });
    dbContext.TaskTags.Add(new ChildCareConnect.Models.TaskTag { TaskId = tasks[3].Id, TagId = tags[2].Id });

    await dbContext.SaveChangesAsync();
}
