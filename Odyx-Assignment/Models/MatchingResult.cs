using System.ComponentModel.DataAnnotations;

namespace Odyx.Assignment.Models;

public class MatchingResult
{
    [Required]
    public Provider Provider { get; init; } = default!;

    public decimal MatchScore { get; init; }

    [Range(1, 3)]   
    public int Rank { get; init; }
}

