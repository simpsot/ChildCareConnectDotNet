using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Models;

namespace ChildCareConnect.Services;

public class HouseholdMemberService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public HouseholdMemberService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<HouseholdMember>> GetHouseholdMembersByClientIdAsync(string clientId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.HouseholdMembers
            .Include(h => h.Relationship)
            .Where(h => h.ClientId == clientId)
            .OrderBy(h => h.CreatedAt)
            .ToListAsync();
    }

    public async Task<HouseholdMember?> GetHouseholdMemberByIdAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.HouseholdMembers
            .Include(h => h.Relationship)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<HouseholdMember> CreateHouseholdMemberAsync(HouseholdMember member)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        member.Id = Guid.NewGuid().ToString();
        member.CreatedAt = DateTime.UtcNow;
        context.HouseholdMembers.Add(member);
        await context.SaveChangesAsync();
        return member;
    }

    public async Task<List<HouseholdMember>> CreateHouseholdMembersAsync(List<HouseholdMember> members)
    {
        if (members == null || !members.Any()) return new List<HouseholdMember>();

        await using var context = await _contextFactory.CreateDbContextAsync();
        foreach (var member in members)
        {
            member.Id = Guid.NewGuid().ToString();
            member.CreatedAt = DateTime.UtcNow;
        }
        context.HouseholdMembers.AddRange(members);
        await context.SaveChangesAsync();
        return members;
    }

    public async Task<HouseholdMember?> UpdateHouseholdMemberAsync(string id, HouseholdMember updates)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var member = await context.HouseholdMembers.FindAsync(id);
        if (member == null) return null;

        member.Name = updates.Name;
        member.RelationshipId = updates.RelationshipId;
        member.DateOfBirth = updates.DateOfBirth;
        member.Notes = updates.Notes;

        await context.SaveChangesAsync();
        return member;
    }

    public async Task<bool> DeleteHouseholdMemberAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var member = await context.HouseholdMembers.FindAsync(id);
        if (member == null) return false;

        context.HouseholdMembers.Remove(member);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteHouseholdMembersByClientIdAsync(string clientId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var members = await context.HouseholdMembers
            .Where(h => h.ClientId == clientId)
            .ToListAsync();

        if (!members.Any()) return true;

        context.HouseholdMembers.RemoveRange(members);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task ReplaceHouseholdMembersAsync(string clientId, List<HouseholdMember> newMembers)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var existingMembers = await context.HouseholdMembers
            .Where(h => h.ClientId == clientId)
            .ToListAsync();
        context.HouseholdMembers.RemoveRange(existingMembers);

        foreach (var member in newMembers)
        {
            member.Id = Guid.NewGuid().ToString();
            member.ClientId = clientId;
            member.CreatedAt = DateTime.UtcNow;
        }
        context.HouseholdMembers.AddRange(newMembers);

        await context.SaveChangesAsync();
    }
}
