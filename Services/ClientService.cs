using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Models;

namespace ChildCareConnect.Services;

public class ClientService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ClientService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Client>> GetAllClientsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Clients
            .Include(c => c.CaseManager)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<Client>> GetClientsByCaseManagerAsync(string caseManagerId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Clients
            .Include(c => c.CaseManager)
            .Where(c => c.CaseManagerId == caseManagerId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Client?> GetClientByIdAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Clients
            .Include(c => c.CaseManager)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Client> CreateClientAsync(Client client, Dictionary<string, string>? customFields = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        client.Id = Guid.NewGuid().ToString();
        client.CreatedAt = DateTime.UtcNow;

        // Encrypt SSN before storing
        if (!string.IsNullOrEmpty(client.SSN))
        {
            client.SSN = EncryptionService.EncryptSSN(client.SSN);
        }

        // Format phone number
        if (!string.IsNullOrEmpty(client.PhoneNumber))
        {
            client.PhoneNumber = EncryptionService.FormatPhoneNumber(client.PhoneNumber);
        }

        context.Clients.Add(client);
        await context.SaveChangesAsync();

        if (customFields != null)
        {
            await SaveClientCustomFieldsInternalAsync(context, client.Id, customFields);
        }

        return client;
    }

    public async Task<Client?> UpdateClientAsync(string id, Client updates)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var client = await context.Clients.FindAsync(id);
        if (client == null) return null;

        client.Name = updates.Name;
        client.Contact = updates.Contact;
        client.DateOfBirth = updates.DateOfBirth;
        
        // Only update SSN if a new one is provided
        if (!string.IsNullOrEmpty(updates.SSN) && updates.SSN != client.SSN)
        {
            client.SSN = EncryptionService.EncryptSSN(updates.SSN);
        }

        if (!string.IsNullOrEmpty(updates.PhoneNumber))
        {
            client.PhoneNumber = EncryptionService.FormatPhoneNumber(updates.PhoneNumber);
        }

        client.PhoneType = updates.PhoneType;
        client.Gender = updates.Gender;
        client.Race = updates.Race;
        client.Nationality = updates.Nationality;
        client.CitizenshipStatus = updates.CitizenshipStatus;
        client.HouseholdSize = updates.HouseholdSize;
        client.Status = updates.Status;
        client.LastContact = updates.LastContact;
        client.CaseManagerId = updates.CaseManagerId;

        await context.SaveChangesAsync();
        return client;
    }

    public async Task<Client?> GetClientWithHouseholdMembersAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Clients
            .Include(c => c.CaseManager)
            .Include(c => c.HouseholdMembers)
                .ThenInclude(h => h.Relationship)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Dictionary<string, string>> GetClientCustomFieldsAsync(string clientId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var fields = await context.ClientCustomFields
            .Include(cf => cf.Field)
            .Where(cf => cf.ClientId == clientId)
            .ToListAsync();

        return fields.ToDictionary(
            cf => cf.Field?.FieldName ?? cf.FieldId,
            cf => cf.Value ?? string.Empty
        );
    }

    public async Task SaveClientCustomFieldsAsync(string clientId, Dictionary<string, string> customFields)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await SaveClientCustomFieldsInternalAsync(context, clientId, customFields);
    }

    private async Task SaveClientCustomFieldsInternalAsync(AppDbContext context, string clientId, Dictionary<string, string> customFields)
    {
        var existingFields = await context.ClientCustomFields
            .Where(cf => cf.ClientId == clientId)
            .ToListAsync();
        context.ClientCustomFields.RemoveRange(existingFields);

        var formFields = await context.FormFields
            .Where(f => f.FormType == "client")
            .ToListAsync();

        foreach (var (fieldName, value) in customFields)
        {
            var formField = formFields.FirstOrDefault(f => f.FieldName == fieldName);
            if (formField != null)
            {
                context.ClientCustomFields.Add(new ClientCustomField
                {
                    ClientId = clientId,
                    FieldId = formField.Id,
                    Value = value
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
