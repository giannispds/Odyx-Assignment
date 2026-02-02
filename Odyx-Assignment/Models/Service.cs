using Odyx.Assignment.Enums;
using System.ComponentModel.DataAnnotations;

namespace Odyx.Assignment.Models;

public class Service
{
    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string Domain { get; init; } = string.Empty;

    [Required]
    public string Subdomain { get; init; } = string.Empty;

    [Range(1, 4)]
    public DigitalMaturityLevel MaturityStage { get; init; }
}
