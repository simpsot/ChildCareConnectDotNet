using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildCareConnect.Models;

[Table("clients")]
public class Client
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("contact")]
    public string Contact { get; set; } = string.Empty;

    [Column("children")]
    public int Children { get; set; } = 1;

    [Required]
    [Column("status")]
    public string Status { get; set; } = "Pending"; // Active, Pending, Inactive, On Hold

    [Column("last_contact")]
    public string? LastContact { get; set; }

    [Column("case_manager_id")]
    public string? CaseManagerId { get; set; }

    [ForeignKey("CaseManagerId")]
    public User? CaseManager { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
