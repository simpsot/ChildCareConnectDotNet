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
        
        var maxOrder = await context.FormFields
            .Where(f => f.FormType == field.FormType)
            .MaxAsync(f => (int?)f.Order) ?? -1;
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

        await context.SaveChangesAsync();
        return field;
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
}
