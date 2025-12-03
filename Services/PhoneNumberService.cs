using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Models;

namespace ChildCareConnect.Services;

public class PhoneNumberService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public PhoneNumberService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<PhoneNumber>> GetPhoneNumbersByClientAsync(string clientId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Set<PhoneNumber>()
            .Where(p => p.ClientId == clientId)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<PhoneNumber> CreatePhoneNumberAsync(PhoneNumber phoneNumber)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        phoneNumber.Id = Guid.NewGuid().ToString();
        phoneNumber.CreatedAt = DateTime.UtcNow;

        // Format phone number
        if (!string.IsNullOrEmpty(phoneNumber.Phone))
        {
            phoneNumber.Phone = EncryptionService.FormatPhoneNumber(phoneNumber.Phone);
        }

        context.Set<PhoneNumber>().Add(phoneNumber);
        await context.SaveChangesAsync();
        return phoneNumber;
    }

    public async Task<bool> DeletePhoneNumberAsync(string phoneNumberId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var phoneNumber = await context.Set<PhoneNumber>().FindAsync(phoneNumberId);
        if (phoneNumber == null) return false;

        context.Set<PhoneNumber>().Remove(phoneNumber);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task CreatePhoneNumbersAsync(List<PhoneNumber> phoneNumbers)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        foreach (var phone in phoneNumbers)
        {
            phone.Id = Guid.NewGuid().ToString();
            phone.CreatedAt = DateTime.UtcNow;
            
            if (!string.IsNullOrEmpty(phone.Phone))
            {
                phone.Phone = EncryptionService.FormatPhoneNumber(phone.Phone);
            }
        }

        context.Set<PhoneNumber>().AddRange(phoneNumbers);
        await context.SaveChangesAsync();
    }
}
