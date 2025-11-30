using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Models;

namespace ChildCareConnect.Services;

public class ClientService
{
    private readonly AppDbContext _context;

    public ClientService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Client>> GetAllClientsAsync()
    {
        return await _context.Clients
            .Include(c => c.CaseManager)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<Client>> GetClientsByCaseManagerAsync(string caseManagerId)
    {
        return await _context.Clients
            .Include(c => c.CaseManager)
            .Where(c => c.CaseManagerId == caseManagerId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Client?> GetClientByIdAsync(string id)
    {
        return await _context.Clients
            .Include(c => c.CaseManager)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Client> CreateClientAsync(Client client, Dictionary<string, string>? customFields = null)
    {
        client.Id = Guid.NewGuid().ToString();
        client.CreatedAt = DateTime.UtcNow;
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        if (customFields != null)
        {
            await SaveClientCustomFieldsAsync(client.Id, customFields);
        }

        return client;
    }

    public async Task<Client?> UpdateClientAsync(string id, Client updates)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return null;

        client.Name = updates.Name;
        client.Contact = updates.Contact;
        client.Children = updates.Children;
        client.Status = updates.Status;
        client.LastContact = updates.LastContact;
        client.CaseManagerId = updates.CaseManagerId;

        await _context.SaveChangesAsync();
        return client;
    }

    public async Task<Dictionary<string, string>> GetClientCustomFieldsAsync(string clientId)
    {
        var fields = await _context.ClientCustomFields
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
        var existingFields = await _context.ClientCustomFields
            .Where(cf => cf.ClientId == clientId)
            .ToListAsync();
        _context.ClientCustomFields.RemoveRange(existingFields);

        var formFields = await _context.FormFields
            .Where(f => f.FormType == "client")
            .ToListAsync();

        foreach (var (fieldName, value) in customFields)
        {
            var formField = formFields.FirstOrDefault(f => f.FieldName == fieldName);
            if (formField != null)
            {
                _context.ClientCustomFields.Add(new ClientCustomField
                {
                    ClientId = clientId,
                    FieldId = formField.Id,
                    Value = value
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}
