using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Models;

namespace ChildCareConnect.Services;

public class FormFieldService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public FormFieldService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<FormField>> GetFieldsByFormTypeAsync(string formType)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.FormFields
            .Where(f => f.FormType == formType)
            .OrderBy(f => f.Order)
            .ToListAsync();
    }

    public async Task<List<FormField>> GetFieldsBySectionAsync(string sectionId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.FormFields
            .Where(f => f.SectionId == sectionId)
            .OrderBy(f => f.Order)
            .ToListAsync();
    }

    public async Task<List<FormField>> GetVisibleFieldsAsync(string formType)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.FormFields
            .Where(f => f.FormType == formType && f.IsVisible)
            .OrderBy(f => f.Order)
            .ToListAsync();
    }

    public async Task<List<FormField>> GetCustomFieldsAsync(string formType)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.FormFields
            .Where(f => f.FormType == formType && f.IsSystem != "true" && f.IsVisible)
            .OrderBy(f => f.Order)
            .ToListAsync();
    }

    public async Task<FormField?> GetFieldByIdAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.FormFields.FindAsync(id);
    }

    public async Task<FormField> CreateFieldAsync(FormField field)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        field.Id = Guid.NewGuid().ToString();
        field.CreatedAt = DateTime.UtcNow;
        
        int maxOrder;
        if (field.SectionId != null)
        {
            maxOrder = await context.FormFields
                .Where(f => f.SectionId == field.SectionId)
                .MaxAsync(f => (int?)f.Order) ?? -1;
        }
        else
        {
            maxOrder = await context.FormFields
                .Where(f => f.FormType == field.FormType && f.SectionId == null)
                .MaxAsync(f => (int?)f.Order) ?? -1;
        }
        field.Order = maxOrder + 1;

        context.FormFields.Add(field);
        await context.SaveChangesAsync();
        return field;
    }

    public async Task<FormField?> UpdateFieldAsync(string id, FormField updates)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var field = await context.FormFields.FindAsync(id);
        if (field == null) return null;

        field.FieldName = updates.FieldName;
        field.FieldLabel = updates.FieldLabel;
        field.FieldType = updates.FieldType;
        field.Options = updates.Options;
        field.Required = updates.Required;
        field.Placeholder = updates.Placeholder;
        field.Width = updates.Width;
        field.IsVisible = updates.IsVisible;
        field.HelpText = updates.HelpText;
        field.DefaultValue = updates.DefaultValue;

        await context.SaveChangesAsync();
        return field;
    }

    public async Task<bool> UpdateFieldVisibilityAsync(string id, bool isVisible)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var field = await context.FormFields.FindAsync(id);
        if (field == null) return false;

        field.IsVisible = isVisible;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateFieldSectionAsync(string id, string? sectionId, int order)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var field = await context.FormFields.FindAsync(id);
        if (field == null) return false;

        field.SectionId = sectionId;
        field.Order = order;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteFieldAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var field = await context.FormFields.FindAsync(id);
        if (field == null || field.IsSystem == "true") return false;

        context.FormFields.Remove(field);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task ReorderFieldsAsync(List<string> fieldIds)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        for (int i = 0; i < fieldIds.Count; i++)
        {
            var field = await context.FormFields.FindAsync(fieldIds[i]);
            if (field != null)
            {
                field.Order = i;
            }
        }
        await context.SaveChangesAsync();
    }

    public async Task ReorderFieldsInSectionAsync(string sectionId, List<string> fieldIds)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        for (int i = 0; i < fieldIds.Count; i++)
        {
            var field = await context.FormFields.FindAsync(fieldIds[i]);
            if (field != null)
            {
                field.SectionId = sectionId;
                field.Order = i;
            }
        }
        await context.SaveChangesAsync();
    }
}
