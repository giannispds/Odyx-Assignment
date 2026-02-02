using System.ComponentModel.DataAnnotations;

namespace Odyx.Assignment.Models;

public class ProviderCertification
{
    [Required]
    public Certification Certification { get; init; } = default!;

    [Required]
    public DateTime IssueDate { get; init; }

    public DateTime? ExpiryDate { get; init; }
}
