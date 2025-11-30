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
    }
}
