namespace ChildCareConnect.Models;

public class DashboardWidget
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // stat, action, chart
    public int Order { get; set; }
    public bool Visible { get; set; } = true;
    public int ColSpan { get; set; } = 1; // 1, 2, 3, or 4 columns
}

public class DashboardPreferences
{
    public List<DashboardWidget> Widgets { get; set; } = new();
    
    public static DashboardPreferences GetDefault()
    {
        return new DashboardPreferences
        {
            Widgets = new List<DashboardWidget>
            {
                new() { Id = "total-clients", Title = "Total Clients", Type = "stat", Order = 0, Visible = true, ColSpan = 1 },
                new() { Id = "active-clients", Title = "Active Clients", Type = "stat", Order = 1, Visible = true, ColSpan = 1 },
                new() { Id = "pending-applications", Title = "Pending Applications", Type = "stat", Order = 2, Visible = true, ColSpan = 1 },
                new() { Id = "active-providers", Title = "Active Providers", Type = "stat", Order = 3, Visible = true, ColSpan = 1 },
                new() { Id = "pending-tasks", Title = "Pending Tasks", Type = "stat", Order = 4, Visible = true, ColSpan = 1 },
                new() { Id = "children-placed", Title = "Children Placed", Type = "info", Order = 5, Visible = true, ColSpan = 1 },
                new() { Id = "quick-actions", Title = "Quick Actions", Type = "action", Order = 6, Visible = true, ColSpan = 1 }
            }
        };
    }
}
