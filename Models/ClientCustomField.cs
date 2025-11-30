using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildCareConnect.Models;

[Table("client_custom_fields")]
public class ClientCustomField
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [Column("client_id")]
    public string ClientId { get; set; } = string.Empty;

    [ForeignKey("ClientId")]
    public Client? Client { get; set; }

    [Required]
    [Column("field_id")]
    public string FieldId { get; set; } = string.Empty;

    [ForeignKey("FieldId")]
    public FormField? Field { get; set; }

    [Column("value")]
    public string? Value { get; set; }
}
