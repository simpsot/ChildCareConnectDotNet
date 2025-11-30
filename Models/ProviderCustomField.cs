using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildCareConnect.Models;

[Table("provider_custom_fields")]
public class ProviderCustomField
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [Column("provider_id")]
    public string ProviderId { get; set; } = string.Empty;

    [ForeignKey("ProviderId")]
    public Provider? Provider { get; set; }

    [Required]
    [Column("field_id")]
    public string FieldId { get; set; } = string.Empty;

    [ForeignKey("FieldId")]
    public FormField? Field { get; set; }

    [Column("value")]
    public string? Value { get; set; }
}
