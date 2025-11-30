using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildCareConnect.Models;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("role")]
    public string Role { get; set; } = "Coordinator"; // Admin, Manager, Case Manager, Coordinator

    [Required]
    [Column("team")]
    public string Team { get; set; } = string.Empty;

    [Required]
    [Column("status")]
    public string Status { get; set; } = "Active"; // Active, Inactive, Away

    [Required]
    [Column("avatar")]
    public string Avatar { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool CanManageUsers => Role == "Admin" || Role == "Manager";
}
