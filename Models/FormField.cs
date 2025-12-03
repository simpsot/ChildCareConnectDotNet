using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildCareConnect.Models;

[Table("form_fields")]
public class FormField
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [Column("form_type")]
    public string FormType { get; set; } = string.Empty;

    [Column("section_id")]
    public string? SectionId { get; set; }

    [Required]
    [Column("field_name")]
    public string FieldName { get; set; } = string.Empty;

    [Required]
    [Column("field_label")]
    public string FieldLabel { get; set; } = string.Empty;

    [Required]
    [Column("field_type")]
    public string FieldType { get; set; } = "text";

    [Column("options")]
    public string? Options { get; set; }

    [Required]
    [Column("required")]
    public string Required { get; set; } = "false";

    [Column("placeholder")]
    public string? Placeholder { get; set; }

    [Column("order")]
    public int Order { get; set; }

    [Required]
    [Column("width")]
    public string Width { get; set; } = "full";

    [Required]
    [Column("is_system")]
    public string IsSystem { get; set; } = "false";

    [Required]
    [Column("is_visible")]
    public bool IsVisible { get; set; } = true;

    [Column("model_property")]
    public string? ModelProperty { get; set; }

    [Column("default_value")]
    public string? DefaultValue { get; set; }

    [Column("validation_regex")]
    public string? ValidationRegex { get; set; }

    [Column("help_text")]
    public string? HelpText { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("SectionId")]
    public virtual FormSection? Section { get; set; }
}
