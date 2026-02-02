using Moq;
using Odyx.Assignment.Enums;
using Odyx.Assignment.Models;
using Odyx.Assignment.Services.Services;

namespace Odyx.Assignment.Tests.Services;

public class MatchingServiceTests
{
    [Fact]
    public void FindTopProviders_ExactMatch_ReturnsProvider()
    {

        // Arrange
        var service = CreateServiceWithFixedScore(7);
        var request = CreateBaseRequest();

        var providers = new List<Provider>
        {
            CreateMatchingProvider()
        };

        // Act
        var results = service.FindTopProviders(request, providers);

        // Assert
        Assert.Single(results);
        Assert.Equal(1, results[0].Rank);
        Assert.Equal(7, results[0].MatchScore);
    }

    [Fact]
    public void FindTopProviders_NoProviders_ReturnsEmptyList()
    {
        // Arrange
        var service = CreateServiceWithFixedScore(5);

        // Act
        var results = service.FindTopProviders(
            new MatchingRequest(),
            new List<Provider>());

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void FindTopProviders_LessThanThreeProviders_ReturnsAll()
    {
        // Arrange
        var service = CreateServiceWithFixedScore(5);
        var request = CreateBaseRequest();

        var providers = new List<Provider>
        {
            CreateMatchingProvider(),
            CreateMatchingProvider()
        };

        //Act
        var results = service.FindTopProviders(request, providers);

        //Assert
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void FindTopProviders_OrdersByScoreDescending()
    {
        // Arrange
        var scoringMock = new Mock<IProviderScoringService>();

        scoringMock.SetupSequence(s => s.CalculateProviderScore(
                It.IsAny<Provider>(),
                It.IsAny<List<Certification>>()))
            .Returns(3)
            .Returns(9);

        var service = new MatchingService(scoringMock.Object);
        var request = CreateBaseRequest();

        // Act
        var providers = new List<Provider>
        {
            CreateMatchingProvider(),
            CreateMatchingProvider()
        };

        var results = service.FindTopProviders(request, providers);

        // Assert
        Assert.Equal(9, results[0].MatchScore);
        Assert.Equal(1, results[0].Rank);
        Assert.Equal(2, results[1].Rank);
    }

    [Fact]
    public void FindTopProviders_FiltersByUserCapacity()
    {
        // Arrange
        var service = CreateServiceWithFixedScore(5);
        var request = new MatchingRequest
        {
            Requestor = new Requestor
            {
                CostProfile = CostProfile.Medium,
                Location = "Athens"
            },
            RequiredService = new Service
            {
                Name = "Cloud",
                MaturityStage = DigitalMaturityLevel.Level3
            },
            NumberOfUsers = 100
        };

        // Act
        var providers = new List<Provider>
        {
            CreateMatchingProvider(maxUsers: 50),   
            CreateMatchingProvider(maxUsers: 200) 
        };

        var results = service.FindTopProviders(request, providers);

        // Assert
        Assert.Single(results);
    }

    [Fact]
    public void FindTopProviders_FiltersByCostProfile()
    {
        // Arrange
        var service = CreateServiceWithFixedScore(5);
        var request = new MatchingRequest
        {
            Requestor = new Requestor
            {
                CostProfile = CostProfile.Medium,
                Location = "Athens"
            },
            RequiredService = new Service
            {
                Name = "Cloud",
                MaturityStage = DigitalMaturityLevel.Level3
            },
            NumberOfUsers = 100
        };
        // Act
        var providers = new List<Provider>
        {
            CreateMatchingProvider(size: ProviderSize.Big),
            CreateMatchingProvider(size: ProviderSize.Small)
        };

        var results = service.FindTopProviders(request, providers);
        // Assert
        Assert.Single(results);
    }

                       // --- HELPERS ---

    // Creates a MatchingService instance with a fixed score
    private static MatchingService CreateServiceWithFixedScore(decimal score)
    {
        var scoringMock = new Mock<IProviderScoringService>();
        scoringMock.Setup(s => s.CalculateProviderScore(
                It.IsAny<Provider>(),
                It.IsAny<List<Certification>>()))
            .Returns(score);

        return new MatchingService(scoringMock.Object);
    }

    // Creates a base matching request with common required values
    private static MatchingRequest CreateBaseRequest() =>
        new()
        {
            Requestor = new Requestor
            {
                CostProfile = CostProfile.Medium,
                Location = "Athens"
            },
            RequiredService = new Service
            {
                Name = "Cloud",
                MaturityStage = DigitalMaturityLevel.Level3
            }
        };

    // Creates a provider that matches the base request by default
    private static Provider CreateMatchingProvider(
        int maxUsers = 200,
        ProviderSize size = ProviderSize.SME) =>
        new()
        {
            Location = "Athens",
            Size = size,
            Skills = new List<ProviderSkill>
            {
                new ProviderSkill
                {
                    MaxUsersSupported = maxUsers,
                    Service = new Service
                    {
                        Name = "Cloud",
                        MaturityStage = DigitalMaturityLevel.Level3
                    }
                }
            }
        };
}
