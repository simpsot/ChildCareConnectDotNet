using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Models;

namespace ChildCareConnect.Services;

public class UserService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public UserService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.OrderBy(u => u.Name).ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        user.Id = Guid.NewGuid().ToString();
        user.CreatedAt = DateTime.UtcNow;
        user.Avatar = GetInitials(user.Name);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateUserAsync(string id, User updates)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = await context.Users.FindAsync(id);
        if (user == null) return null;

        user.Name = updates.Name;
        user.Email = updates.Email;
        user.Role = updates.Role;
        user.Team = updates.Team;
        user.Status = updates.Status;
        user.Avatar = GetInitials(updates.Name);

        await context.SaveChangesAsync();
        return user;
    }

    public async Task<DashboardPreferences> GetDashboardPreferencesAsync(string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = await context.Users.FindAsync(userId);
        return user?.DashboardPreferences ?? DashboardPreferences.GetDefault();
    }

    public async Task<bool> SaveDashboardPreferencesAsync(string userId, DashboardPreferences preferences)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = await context.Users.FindAsync(userId);
        if (user == null) return false;

        user.DashboardPreferences = preferences;
        await context.SaveChangesAsync();
        return true;
    }

    private static string GetInitials(string name)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0][0]}{parts[1][0]}".ToUpper();
        return parts.Length > 0 ? parts[0][..Math.Min(2, parts[0].Length)].ToUpper() : "??";
    }
}
