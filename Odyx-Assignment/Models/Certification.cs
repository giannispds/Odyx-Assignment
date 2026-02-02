using System.ComponentModel.DataAnnotations;

namespace Odyx.Assignment.Models;

public class Certification
{
    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string IssuingOrganization { get; init; } = string.Empty;
}
