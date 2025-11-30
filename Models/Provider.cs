using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildCareConnect.Models;

[Table("providers")]
public class Provider
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("type")]
    public string Type { get; set; } = string.Empty; // Center, In-Home, Preschool

    [Column("capacity")]
    public int Capacity { get; set; }

    [Column("enrollment")]
    public int Enrollment { get; set; }

    [Required]
    [Column("rating")]
    public string Rating { get; set; } = string.Empty;

    [Required]
    [Column("status")]
    public string Status { get; set; } = "Pending"; // Verified, Pending, Review Needed

    [Required]
    [Column("location")]
    public string Location { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
