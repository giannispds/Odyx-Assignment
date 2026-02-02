using System.ComponentModel.DataAnnotations;

namespace Odyx.Assignment.Models;

public class ProviderSkill
{
    [Required]
    public Service Service { get; init; } = default!;

    [Range(0, int.MaxValue)]
    public int? MaxUsersSupported { get; init; }

    [Range(0, 100)]
    public int YearsOfExperience { get; init; }
}
