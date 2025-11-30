using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Models;

namespace ChildCareConnect.Services;

public class ProviderService
{
    private readonly AppDbContext _context;

    public ProviderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Provider>> GetAllProvidersAsync()
    {
        return await _context.Providers.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Provider?> GetProviderByIdAsync(string id)
    {
        return await _context.Providers.FindAsync(id);
    }

    public async Task<Provider> CreateProviderAsync(Provider provider, Dictionary<string, string>? customFields = null)
    {
        provider.Id = Guid.NewGuid().ToString();
        provider.CreatedAt = DateTime.UtcNow;
        _context.Providers.Add(provider);
        await _context.SaveChangesAsync();

        if (customFields != null)
        {
            await SaveProviderCustomFieldsAsync(provider.Id, customFields);
        }

        return provider;
    }

    public async Task<Provider?> UpdateProviderAsync(string id, Provider updates)
    {
        var provider = await _context.Providers.FindAsync(id);
        if (provider == null) return null;

        provider.Name = updates.Name;
        provider.Type = updates.Type;
        provider.Capacity = updates.Capacity;
        provider.Enrollment = updates.Enrollment;
        provider.Rating = updates.Rating;
        provider.Status = updates.Status;
        provider.Location = updates.Location;

        await _context.SaveChangesAsync();
        return provider;
    }

    public async Task<Dictionary<string, string>> GetProviderCustomFieldsAsync(string providerId)
    {
        var fields = await _context.ProviderCustomFields
            .Include(pf => pf.Field)
            .Where(pf => pf.ProviderId == providerId)
            .ToListAsync();

        return fields.ToDictionary(
            pf => pf.Field?.FieldName ?? pf.FieldId,
            pf => pf.Value ?? string.Empty
        );
    }

    public async Task SaveProviderCustomFieldsAsync(string providerId, Dictionary<string, string> customFields)
    {
        var existingFields = await _context.ProviderCustomFields
            .Where(pf => pf.ProviderId == providerId)
            .ToListAsync();
        _context.ProviderCustomFields.RemoveRange(existingFields);

        var formFields = await _context.FormFields
            .Where(f => f.FormType == "provider")
            .ToListAsync();

        foreach (var (fieldName, value) in customFields)
        {
            var formField = formFields.FirstOrDefault(f => f.FieldName == fieldName);
            if (formField != null)
            {
                _context.ProviderCustomFields.Add(new ProviderCustomField
                {
                    ProviderId = providerId,
                    FieldId = formField.Id,
                    Value = value
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}
