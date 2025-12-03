using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildCareConnect.Models;

[Table("phone_numbers")]
public class PhoneNumber
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
    [Column("phone")]
    public string Phone { get; set; } = string.Empty; // Formatted as (XXX) XXX-XXXX

    [Column("phone_type")]
    public string PhoneType { get; set; } = "Main"; // Mobile, Main, Work, Other

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
