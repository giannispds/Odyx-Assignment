using Odyx.Assignment.Models;
using Odyx.Assignment.Services.Services;

namespace Odyx.Assignment.Tests.Services;

public class ProviderScoringServiceTests
{
    // Test with all factors present
    [Fact]
    public void CalculateProviderScore_AllFactorsPresent_ReturnsCorrectAverage()
    {
        // Arrange
        var provider = new Provider
        {
            AssessmentScore = 4.5m,
            LastActivityDate = DateTime.UtcNow.AddMonths(-1),
            ProjectCount = 60,
            AverageProjectValue = 300_000,
            Certifications = new List<ProviderCertification>
            {
                new ProviderCertification
                {
                    Certification = new Certification
                    {
                        Name = "ISO",
                        IssuingOrganization = "Org"
                    }
                }
            }
        };

        var relevantCertifications = new List<Certification>
        {
            new Certification
            {
                Name = "ISO",
                IssuingOrganization = "Org"
            }
        };

        var service = new ProviderScoringService();

        // Act
        var score = service.CalculateProviderScore(
            provider,
            relevantCertifications);

        // (9 + 6 + 12 + 6 + 9) / 5 = 8.4
        Assert.Equal(8.4m, score);
    }

    // Test with missing factors
    [Fact]
    public void CalculateProviderScore_MissingFactors_ReturnsMinimalScore()
    {
        // Arrange
        var provider = new Provider
        {
            AssessmentScore = -1,
            ProjectCount = -1,
            AverageProjectValue = 0,
            LastActivityDate = null,
            Certifications = null
        };

        var service = new ProviderScoringService();

        // Act
        var score = service.CalculateProviderScore(
            provider,
            new List<Certification>());

        // Assert
        Assert.Equal(1, score);
    }

    // Test edge cases (boundary values)
    [Theory]
    [InlineData(2.25, 1.0)]
    [InlineData(2.26, 2.0)]
    [InlineData(3.75, 2.0)]
    [InlineData(3.76, 5.0)]
    public void CalculateProviderScore_AssessmentScoreBoundaries_AffectAverageCorrectly(
        decimal assessmentScore,
        double expectedAverage)
    {
        // Arrange
        var provider = new Provider
        {
            AssessmentScore = assessmentScore,
            Certifications = null,  
            ProjectCount = -1,        
            AverageProjectValue = 0, 
            LastActivityDate = null    
        };

        var service = new ProviderScoringService();

        // Act
        var score = service.CalculateProviderScore(
            provider,
            new List<Certification>());

        // Assert
        Assert.Equal((decimal)expectedAverage, score);
    }
}
