using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildCareConnect.Models;

[Table("clients")]
public class Client
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("contact")]
    public string Contact { get; set; } = string.Empty;

    [Column("date_of_birth")]
    public DateTime? DateOfBirth { get; set; }

    [Column("ssn")]
    public string? SSN { get; set; } // Encrypted in database

    [Column("phone_number")]
    public string? PhoneNumber { get; set; } // Formatted as (XXX) XXX-XXXX

    [Column("phone_type")]
    public string? PhoneType { get; set; } = "Main"; // Mobile, Main

    [Column("gender")]
    public string? Gender { get; set; } // Male, Female, Non-binary, Other

    [Column("race")]
    public string? Race { get; set; }

    [Column("nationality")]
    public string? Nationality { get; set; }

    [Column("citizenship_status")]
    public string? CitizenshipStatus { get; set; } // U.S. Citizen, Permanent Resident, etc.

    [Column("household_size")]
    public int HouseholdSize { get; set; } = 1;

    [Required]
    [Column("status")]
    public string Status { get; set; } = "Pending"; // Active, Pending, Inactive, On Hold

    [Column("last_contact")]
    public string? LastContact { get; set; }

    [Column("case_manager_id")]
    public string? CaseManagerId { get; set; }

    [ForeignKey("CaseManagerId")]
    public User? CaseManager { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<HouseholdMember> HouseholdMembers { get; set; } = new List<HouseholdMember>();

    [NotMapped]
    public string MaskedSSN => string.IsNullOrEmpty(SSN) ? "" : $"XXX-XX-{SSN[^4..]}";
}
