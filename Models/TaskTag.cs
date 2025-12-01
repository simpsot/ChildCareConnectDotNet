using System.ComponentModel.DataAnnotations.Schema;

namespace ChildCareConnect.Models;

[Table("task_tags")]
public class TaskTag
{
    [Column("task_id")]
    public string TaskId { get; set; } = string.Empty;

    public TaskItem? Task { get; set; }

    [Column("tag_id")]
    public string TagId { get; set; } = string.Empty;

    public Tag? Tag { get; set; }

    [Column("added_at")]
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
