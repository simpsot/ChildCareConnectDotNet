using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Models;

namespace ChildCareConnect.Services;

public class RelationshipService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public RelationshipService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Relationship>> GetAllRelationshipsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Relationships
            .Where(r => r.IsActive)
            .OrderBy(r => r.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Relationship?> GetRelationshipByIdAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Relationships.FindAsync(id);
    }

    public async Task<Relationship> CreateRelationshipAsync(Relationship relationship)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        relationship.Id = Guid.NewGuid().ToString();
        relationship.CreatedAt = DateTime.UtcNow;
        context.Relationships.Add(relationship);
        await context.SaveChangesAsync();
        return relationship;
    }

    public async Task<Relationship?> UpdateRelationshipAsync(string id, Relationship updates)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var relationship = await context.Relationships.FindAsync(id);
        if (relationship == null) return null;

        relationship.Name = updates.Name;
        relationship.DisplayOrder = updates.DisplayOrder;
        relationship.IsActive = updates.IsActive;

        await context.SaveChangesAsync();
        return relationship;
    }

    public async Task<bool> DeleteRelationshipAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var relationship = await context.Relationships.FindAsync(id);
        if (relationship == null) return false;

        relationship.IsActive = false;
        await context.SaveChangesAsync();
        return true;
    }
}
