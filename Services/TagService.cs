using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Models;

namespace ChildCareConnect.Services;

public class TagService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public TagService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Tag>> GetAllTagsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Tags
            .Include(t => t.CreatedBy)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Tag?> GetTagByIdAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Tags
            .Include(t => t.CreatedBy)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Tags
            .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
    }

    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var existing = await context.Tags
            .FirstOrDefaultAsync(t => t.Name.ToLower() == tag.Name.ToLower());
        if (existing != null)
        {
            throw new InvalidOperationException($"A tag with the name '{tag.Name}' already exists.");
        }

        tag.Id = Guid.NewGuid().ToString();
        tag.CreatedAt = DateTime.UtcNow;
        context.Tags.Add(tag);
        await context.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag?> UpdateTagAsync(string id, Tag updates)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var tag = await context.Tags.FindAsync(id);
        if (tag == null) return null;

        var existing = await context.Tags
            .FirstOrDefaultAsync(t => t.Name.ToLower() == updates.Name.ToLower() && t.Id != id);
        if (existing != null)
        {
            throw new InvalidOperationException($"A tag with the name '{updates.Name}' already exists.");
        }

        tag.Name = updates.Name;
        tag.Color = updates.Color;
        await context.SaveChangesAsync();
        return tag;
    }

    public async Task<bool> DeleteTagAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var tag = await context.Tags.FindAsync(id);
        if (tag == null) return false;

        context.Tags.Remove(tag);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetTagUsageCountAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.TaskTags.CountAsync(tt => tt.TagId == id);
    }
}
