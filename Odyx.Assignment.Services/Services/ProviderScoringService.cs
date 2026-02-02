using Odyx.Assignment.Models;

namespace Odyx.Assignment.Services.Services;

public interface IProviderScoringService
{
    decimal CalculateProviderScore(Provider provider, List<Certification> certifications);
}

public class ProviderScoringService : IProviderScoringService
{
    // Calculates the provider score based on multiple independent business factors
    public decimal CalculateProviderScore( Provider provider, List<Certification> certifications)
    {
        if (provider == null)
            return 0;

        var scores = new List<decimal>();

        AddCertificationScore(provider.Certifications, certifications, scores);
        AddAssessmentScore(provider.AssessmentScore, scores);
        AddRecencyScore(provider.LastActivityDate, scores);
        AddFrequencyScore(provider.ProjectCount, scores);
        AddMonetaryScore(provider.AverageProjectValue, scores);

        // Return zero when no scoring factors can be evaluated
        if (!scores.Any())
            return 0;

        return scores.Average();
    }

    private static void AddCertificationScore(List<ProviderCertification> providerCerts, List<Certification> relevantCerts,
        ICollection<decimal> scores)
    {
        if (providerCerts == null || !providerCerts.Any())
        {
            scores.Add(1); // No certifications
            return;
        }

        bool hasRelevant = false;
        if (relevantCerts != null && relevantCerts.Any())
        {
            hasRelevant = providerCerts.Any(pc =>
                relevantCerts.Any(rc => rc.Name == pc.Certification.Name));
        }
        else
        {
            hasRelevant = providerCerts.Any();
        }

        scores.Add(hasRelevant ? 9 : 1);
    }

    private static void AddAssessmentScore(decimal assessmentScore,ICollection<decimal> scores)
    {
        if (assessmentScore < 0)
            return;

        if (assessmentScore < 2.26m)
            scores.Add(1);
        else if (assessmentScore < 3.76m)
            scores.Add(3);
        else
            scores.Add(9);
    }

    private static void AddRecencyScore( DateTime? lastActivityDate, ICollection<decimal> scores)
    {
        if (!lastActivityDate.HasValue)
            return;

        var monthsAgo =
            (DateTime.UtcNow - lastActivityDate.Value).TotalDays / 30;

        if (monthsAgo > 12)
            scores.Add(1);
        else if (monthsAgo >= 6)
            scores.Add(3);
        else
            scores.Add(6);
    }

    private static void AddFrequencyScore( int projectCount,ICollection<decimal> scores)
    {
        if (projectCount < 0)
            return;

        if (projectCount < 24)
            scores.Add(1);
        else if (projectCount <= 48)
            scores.Add(6);
        else
            scores.Add(12);
    }

    private static void AddMonetaryScore( decimal averageProjectValue, ICollection<decimal> scores)
    {
        if (averageProjectValue <= 0)
            return;

        if (averageProjectValue < 100000)
            scores.Add(1);
        else if (averageProjectValue <= 250000)
            scores.Add(3);
        else
            scores.Add(6);
    }
}
