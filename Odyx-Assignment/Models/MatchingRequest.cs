using System.ComponentModel.DataAnnotations;

namespace Odyx.Assignment.Models;

public class MatchingRequest
{
    [Required]
    public Requestor Requestor { get; init; } = default!;

    [Required]
    public Service RequiredService { get; init; } = default!;

    public int? NumberOfUsers { get; init; }

    public bool RequireLocationProximity { get; init; }
}
