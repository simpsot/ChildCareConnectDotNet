using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Models;

namespace ChildCareConnect.Services;

public class FormSectionService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public FormSectionService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<FormSection>> GetSectionsByFormTypeAsync(string formType)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.FormSections
            .Where(s => s.FormType == formType)
            .OrderBy(s => s.Order)
            .ToListAsync();
    }

    public async Task<List<FormSection>> GetSectionsWithFieldsAsync(string formType)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.FormSections
            .Where(s => s.FormType == formType)
            .Include(s => s.Fields.OrderBy(f => f.Order))
            .OrderBy(s => s.Order)
            .ToListAsync();
    }

    public async Task<FormSection?> GetSectionByIdAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.FormSections
            .Include(s => s.Fields)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<FormSection> CreateSectionAsync(FormSection section)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        section.Id = Guid.NewGuid().ToString();
        section.CreatedAt = DateTime.UtcNow;

        var maxOrder = await context.FormSections
            .Where(s => s.FormType == section.FormType)
            .MaxAsync(s => (int?)s.Order) ?? -1;
        section.Order = maxOrder + 1;

        context.FormSections.Add(section);
        await context.SaveChangesAsync();
        return section;
    }

    public async Task<FormSection?> UpdateSectionAsync(string id, FormSection updates)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var section = await context.FormSections.FindAsync(id);
        if (section == null) return null;

        section.Name = updates.Name;
        section.Description = updates.Description;
        section.IsVisible = updates.IsVisible;
        section.IsCollapsible = updates.IsCollapsible;
        section.Icon = updates.Icon;

        await context.SaveChangesAsync();
        return section;
    }

    public async Task<bool> DeleteSectionAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var section = await context.FormSections.FindAsync(id);
        if (section == null || section.IsSystem) return false;

        context.FormSections.Remove(section);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task ReorderSectionsAsync(List<string> sectionIds)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        for (int i = 0; i < sectionIds.Count; i++)
        {
            var section = await context.FormSections.FindAsync(sectionIds[i]);
            if (section != null)
            {
                section.Order = i;
            }
        }
        await context.SaveChangesAsync();
    }

    public async Task MoveFieldToSectionAsync(string fieldId, string? sectionId, int newOrder)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var field = await context.FormFields.FindAsync(fieldId);
        if (field == null) return;

        field.SectionId = sectionId;
        field.Order = newOrder;

        await context.SaveChangesAsync();
    }
}
