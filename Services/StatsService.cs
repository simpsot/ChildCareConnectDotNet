using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;

namespace ChildCareConnect.Services;

public class StatsService
{
    private readonly AppDbContext _context;

    public StatsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStats> GetDashboardStatsAsync(string? caseManagerId = null, bool isAdmin = true)
    {
        var clientsQuery = _context.Clients.AsQueryable();
        var providersQuery = _context.Providers.AsQueryable();

        if (!isAdmin && !string.IsNullOrEmpty(caseManagerId))
        {
            clientsQuery = clientsQuery.Where(c => c.CaseManagerId == caseManagerId);
        }

        var clients = await clientsQuery.ToListAsync();
        var providers = await providersQuery.ToListAsync();

        return new DashboardStats
        {
            TotalClients = clients.Count,
            ActiveClients = clients.Count(c => c.Status == "Active"),
            PendingApplications = clients.Count(c => c.Status == "Pending"),
            ActiveProviders = providers.Count(p => p.Status == "Verified"),
            ChildrenPlaced = clients.Sum(c => c.Children)
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
}
