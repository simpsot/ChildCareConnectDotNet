using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildCareConnect.Models;

[Table("tags")]
public class Tag
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("color")]
    public string Color { get; set; } = "gray";

    [Column("created_by_id")]
    public string? CreatedById { get; set; }

    [ForeignKey("CreatedById")]
    public User? CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<TaskTag> TaskTags { get; set; } = new();
}

public static class TagColors
{
    public static readonly string[] All = { "gray", "red", "orange", "yellow", "green", "teal", "blue", "indigo", "purple", "pink" };
}
