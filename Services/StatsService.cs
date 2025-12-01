using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;

namespace ChildCareConnect.Services;

public class StatsService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public StatsService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<DashboardStats> GetDashboardStatsAsync(string? userId = null, bool isAdmin = true)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var clientsQuery = context.Clients.AsQueryable();
        var providersQuery = context.Providers.AsQueryable();

        if (!isAdmin && !string.IsNullOrEmpty(userId))
        {
            clientsQuery = clientsQuery.Where(c => c.CaseManagerId == userId);
        }

        var clients = await clientsQuery.ToListAsync();
        var providers = await providersQuery.ToListAsync();

        var pendingTasks = 0;
        if (!string.IsNullOrEmpty(userId))
        {
            pendingTasks = await context.Tasks.CountAsync(t => t.AssigneeId == userId && t.Status != "Completed");
        }
        else
        {
            pendingTasks = await context.Tasks.CountAsync(t => t.Status != "Completed");
        }

        return new DashboardStats
        {
            TotalClients = clients.Count,
            ActiveClients = clients.Count(c => c.Status == "Active"),
            PendingApplications = clients.Count(c => c.Status == "Pending"),
            ActiveProviders = providers.Count(p => p.Status == "Verified"),
            ChildrenPlaced = clients.Sum(c => c.Children),
            PendingTasks = pendingTasks
        };
    }
}

public class DashboardStats
{
    public int TotalClients { get; set; }
    public int ActiveClients { get; set; }
    public int PendingApplications { get; set; }
    public int ActiveProviders { get; set; }
    public int ChildrenPlaced { get; set; }
    public int PendingTasks { get; set; }
}
