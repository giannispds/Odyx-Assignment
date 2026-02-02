using Odyx.Assignment.Enums;
using System.ComponentModel.DataAnnotations;

namespace Odyx.Assignment.Models;

public class Provider
{
    [Required]
    public string CompanyName { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int EmployeeCount { get; init; }

    [Required]
    public string Location { get; init; } = string.Empty;

    [Range(0, 5)]
    public decimal AssessmentScore { get; init; }
    public DateTime? LastActivityDate { get; init; }

    [Range(0, int.MaxValue)]
    public int ProjectCount { get; init; }

    [Range(0, double.MaxValue)]
    public decimal AverageProjectValue { get; init; }

    public ProviderSize Size { get; init; }

    public List<ProviderSkill> Skills { get; init; } = new();

    public List<ProviderCertification> Certifications { get; init; } = new();
}
