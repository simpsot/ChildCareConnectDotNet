using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildCareConnect.Models;

[Table("form_sections")]
public class FormSection
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [Column("form_type")]
    public string FormType { get; set; } = "client";

    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("order")]
    public int Order { get; set; }

    [Required]
    [Column("is_system")]
    public bool IsSystem { get; set; } = false;

    [Required]
    [Column("is_visible")]
    public bool IsVisible { get; set; } = true;

    [Required]
    [Column("is_collapsible")]
    public bool IsCollapsible { get; set; } = false;

    [Column("icon")]
    public string? Icon { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<FormField> Fields { get; set; } = new List<FormField>();
}
