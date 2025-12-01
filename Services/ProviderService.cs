using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Models;

namespace ChildCareConnect.Services;

public class ProviderService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ProviderService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Provider>> GetAllProvidersAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Providers.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Provider?> GetProviderByIdAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Providers.FindAsync(id);
    }

    public async Task<Provider> CreateProviderAsync(Provider provider, Dictionary<string, string>? customFields = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        provider.Id = Guid.NewGuid().ToString();
        provider.CreatedAt = DateTime.UtcNow;
        context.Providers.Add(provider);
        await context.SaveChangesAsync();

        if (customFields != null)
        {
            await SaveProviderCustomFieldsInternalAsync(context, provider.Id, customFields);
        }

        return provider;
    }

    public async Task<Provider?> UpdateProviderAsync(string id, Provider updates)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var provider = await context.Providers.FindAsync(id);
        if (provider == null) return null;

        provider.Name = updates.Name;
        provider.Type = updates.Type;
        provider.Capacity = updates.Capacity;
        provider.Enrollment = updates.Enrollment;
        provider.Rating = updates.Rating;
        provider.Status = updates.Status;
        provider.Location = updates.Location;

        await context.SaveChangesAsync();
        return provider;
    }

    public async Task<Dictionary<string, string>> GetProviderCustomFieldsAsync(string providerId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var fields = await context.ProviderCustomFields
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
        await using var context = await _contextFactory.CreateDbContextAsync();
        await SaveProviderCustomFieldsInternalAsync(context, providerId, customFields);
    }

    private async Task SaveProviderCustomFieldsInternalAsync(AppDbContext context, string providerId, Dictionary<string, string> customFields)
    {
        var existingFields = await context.ProviderCustomFields
            .Where(pf => pf.ProviderId == providerId)
            .ToListAsync();
        context.ProviderCustomFields.RemoveRange(existingFields);

        var formFields = await context.FormFields
            .Where(f => f.FormType == "provider")
            .ToListAsync();

        foreach (var (fieldName, value) in customFields)
        {
            var formField = formFields.FirstOrDefault(f => f.FieldName == fieldName);
            if (formField != null)
            {
                context.ProviderCustomFields.Add(new ProviderCustomField
                {
                    ProviderId = providerId,
                    FieldId = formField.Id,
                    Value = value
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
