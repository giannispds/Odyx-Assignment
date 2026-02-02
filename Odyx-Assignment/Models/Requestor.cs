using Odyx.Assignment.Enums;
using System.ComponentModel.DataAnnotations;

namespace Odyx.Assignment.Models;

public class Requestor
{
    [Required]
    public string CompanyName { get; init; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Revenue { get; init; }

    [Range(1, int.MaxValue)]
    public int EmployeeCount { get; init; }

    [Required]
    public CostProfile CostProfile { get; init; }

    [Range(1, 4)]
    public DigitalMaturityLevel DigitalMaturityIndex { get; init; }

    [Required]
    public string Location { get; init; } = string.Empty;
}
