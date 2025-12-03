using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Models;

namespace ChildCareConnect.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Provider> Providers { get; set; } = null!;
    public DbSet<FormField> FormFields { get; set; } = null!;
    public DbSet<ClientCustomField> ClientCustomFields { get; set; } = null!;
    public DbSet<ProviderCustomField> ProviderCustomFields { get; set; } = null!;
    public DbSet<TaskItem> Tasks { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<TaskTag> TaskTags { get; set; } = null!;
    public DbSet<Relationship> Relationships { get; set; } = null!;
    public DbSet<HouseholdMember> HouseholdMembers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Client>()
            .HasOne(c => c.CaseManager)
            .WithMany()
            .HasForeignKey(c => c.CaseManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ClientCustomField>()
            .HasOne(cf => cf.Client)
            .WithMany()
            .HasForeignKey(cf => cf.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClientCustomField>()
            .HasOne(cf => cf.Field)
            .WithMany()
            .HasForeignKey(cf => cf.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProviderCustomField>()
            .HasOne(pf => pf.Provider)
            .WithMany()
            .HasForeignKey(pf => pf.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProviderCustomField>()
            .HasOne(pf => pf.Field)
            .WithMany()
            .HasForeignKey(pf => pf.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Assignee)
            .WithMany()
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Creator)
            .WithMany()
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskItem>()
            .HasIndex(t => new { t.AssigneeId, t.Status, t.DueDate });

        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

        modelBuilder.Entity<Tag>()
            .HasOne(t => t.CreatedBy)
            .WithMany()
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<TaskTag>()
            .HasKey(tt => new { tt.TaskId, tt.TagId });

        modelBuilder.Entity<TaskTag>()
            .HasOne(tt => tt.Task)
            .WithMany(t => t.TaskTags)
            .HasForeignKey(tt => tt.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskTag>()
            .HasOne(tt => tt.Tag)
            .WithMany(t => t.TaskTags)
            .HasForeignKey(tt => tt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Relationship>()
            .HasIndex(r => r.Name)
            .IsUnique();

        modelBuilder.Entity<HouseholdMember>()
            .HasOne(h => h.Client)
            .WithMany(c => c.HouseholdMembers)
            .HasForeignKey(h => h.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HouseholdMember>()
            .HasOne(h => h.Relationship)
            .WithMany()
            .HasForeignKey(h => h.RelationshipId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
