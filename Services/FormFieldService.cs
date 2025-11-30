using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Models;

namespace ChildCareConnect.Services;

public class FormFieldService
{
    private readonly AppDbContext _context;

    public FormFieldService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FormField>> GetFieldsByFormTypeAsync(string formType)
    {
        return await _context.FormFields
            .Where(f => f.FormType == formType)
            .OrderBy(f => f.Order)
            .ToListAsync();
    }

    public async Task<FormField?> GetFieldByIdAsync(string id)
    {
        return await _context.FormFields.FindAsync(id);
    }

    public async Task<FormField> CreateFieldAsync(FormField field)
    {
        field.Id = Guid.NewGuid().ToString();
        field.CreatedAt = DateTime.UtcNow;
        
        var maxOrder = await _context.FormFields
            .Where(f => f.FormType == field.FormType)
            .MaxAsync(f => (int?)f.Order) ?? -1;
        field.Order = maxOrder + 1;

        _context.FormFields.Add(field);
        await _context.SaveChangesAsync();
        return field;
    }

    public async Task<FormField?> UpdateFieldAsync(string id, FormField updates)
    {
        var field = await _context.FormFields.FindAsync(id);
        if (field == null) return null;

        field.FieldName = updates.FieldName;
        field.FieldLabel = updates.FieldLabel;
        field.FieldType = updates.FieldType;
        field.Options = updates.Options;
        field.Required = updates.Required;
        field.Placeholder = updates.Placeholder;
        field.Width = updates.Width;

        await _context.SaveChangesAsync();
        return field;
    }

    public async Task<bool> DeleteFieldAsync(string id)
    {
        var field = await _context.FormFields.FindAsync(id);
        if (field == null || field.IsSystem == "true") return false;

        _context.FormFields.Remove(field);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task ReorderFieldsAsync(List<string> fieldIds)
    {
        for (int i = 0; i < fieldIds.Count; i++)
        {
            var field = await _context.FormFields.FindAsync(fieldIds[i]);
            if (field != null)
            {
                field.Order = i;
            }
        }
        await _context.SaveChangesAsync();
    }
}
