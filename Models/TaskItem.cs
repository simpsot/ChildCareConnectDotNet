using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildCareConnect.Models;

[Table("tasks")]
public class TaskItem
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("priority")]
    public string Priority { get; set; } = "Normal";

    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "Pending";

    [Column("due_date")]
    public DateTime? DueDate { get; set; }

    [Required]
    [Column("assignee_id")]
    public string AssigneeId { get; set; } = string.Empty;

    [ForeignKey("AssigneeId")]
    public User? Assignee { get; set; }

    [Required]
    [Column("creator_id")]
    public string CreatorId { get; set; } = string.Empty;

    [ForeignKey("CreatorId")]
    public User? Creator { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<TaskTag> TaskTags { get; set; } = new();
}

public static class TaskPriority
{
    public const string Low = "Low";
    public const string Normal = "Normal";
    public const string High = "High";
    public const string Urgent = "Urgent";

    public static readonly string[] All = { Low, Normal, High, Urgent };
}

public static class TaskItemStatus
{
    public const string Pending = "Pending";
    public const string InProgress = "In Progress";
    public const string Completed = "Completed";

    public static readonly string[] All = { Pending, InProgress, Completed };
}
