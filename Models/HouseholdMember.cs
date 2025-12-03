using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildCareConnect.Models;

[Table("household_members")]
public class HouseholdMember
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("client_id")]
    public string ClientId { get; set; } = string.Empty;

    [ForeignKey("ClientId")]
    public Client? Client { get; set; }

    [Required]
    [Column("relationship_id")]
    public string RelationshipId { get; set; } = string.Empty;

    [ForeignKey("RelationshipId")]
    public Relationship? Relationship { get; set; }

    [Column("date_of_birth")]
    public DateTime? DateOfBirth { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
