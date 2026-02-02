using Odyx.Assignment.Enums;
using Odyx.Assignment.Models;

namespace Odyx.Assignment.Services.Services;

public interface IMatchingService
{
    List<MatchingResult> FindTopProviders(MatchingRequest request, List<Provider> providers);
}

public class MatchingService : IMatchingService
{
    private const int TopProviderCount = 3;
    private readonly IProviderScoringService _scoringService;

    public MatchingService(IProviderScoringService scoringService)
    {
        _scoringService = scoringService;
    }

    public List<MatchingResult> FindTopProviders(MatchingRequest request, List<Provider> providers)
    {
        if (request == null || request.Requestor == null || request.RequiredService == null)
        {
            return new List<MatchingResult>();
        }

        if (providers == null || providers.Count == 0)
        {
            return new List<MatchingResult>();
        }

        List<Provider> candidates = FilterByCoreCriteria(request, providers);

        if (request.RequireLocationProximity)
        {
            candidates = PreferSameLocation(candidates, request.Requestor.Location);
        }

        if (candidates.Count == 0)
        {
            candidates = FallbackToServiceNameOnly(providers, request.RequiredService.Name.ToLower());
        }

        return RankAndTakeTop(candidates);
    }

    private List<Provider> FilterByCoreCriteria(MatchingRequest request, List<Provider> providers)
    {
        var filteredList = new List<Provider>();
        string requiredName = request.RequiredService.Name.ToLower();
        var requiredMaturity = request.RequiredService.MaturityStage;

        foreach (var provider in providers)
        {
            if (IsCostProfileMatch(request.Requestor.CostProfile, provider.Size))
            {
                if (HasMatchingSkill(provider, requiredName, requiredMaturity, request.NumberOfUsers))
                {
                    filteredList.Add(provider);
                }
            }
        }
        return filteredList;
    }

    private bool HasMatchingSkill(Provider provider, string requiredServiceName, DigitalMaturityLevel requiredMaturity,int? numberOfUsers)
    {
        foreach (var skill in provider.Skills)
        {
            if (skill.Service.Name.ToLower() != requiredServiceName)
                continue;

            if (skill.Service.MaturityStage != requiredMaturity)
                continue;

            if (numberOfUsers > 0 &&
                skill.MaxUsersSupported.HasValue &&
                skill.MaxUsersSupported.Value < numberOfUsers)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private List<Provider> PreferSameLocation(List<Provider> providers, string requestorLocation)
    {
        var localProviders = new List<Provider>();
        foreach (var p in providers)
        {
            if (p.Location == requestorLocation)
            {
                localProviders.Add(p);
            }
        }

        if (localProviders.Count > 0)
        {
            return localProviders;
        }

        return providers;
    }

    private List<Provider> FallbackToServiceNameOnly(List<Provider> providers, string requiredServiceName)
    {
        var fallbackList = new List<Provider>();
        foreach (var p in providers)
        {
            foreach (var s in p.Skills)
            {
                if (s.Service.Name.ToLower() == requiredServiceName)
                {
                    fallbackList.Add(p);
                    break;
                }
            }
        }
        return fallbackList;
    }

    private List<MatchingResult> RankAndTakeTop(List<Provider> providers)
    {
        if (providers == null || providers.Count == 0)
        {
            return new List<MatchingResult>();
        }

        var tempResults = new List<(Provider Provider, decimal Score)>();

        foreach (var provider in providers)
        {
            var certifications = new List<Certification>();
            foreach (var cert in provider.Certifications)
            {
                certifications.Add(cert.Certification);
            }

            var score = _scoringService.CalculateProviderScore(provider, certifications);
            tempResults.Add((provider, score));
        }

        var sorted = tempResults
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Provider.Certifications.Count)
            .Take(TopProviderCount)
            .ToList();

        var finalResults = new List<MatchingResult>();
        var rank = 1;

        foreach (var item in sorted)
        {
            finalResults.Add(new MatchingResult
            {
                Provider = item.Provider,
                MatchScore = item.Score,
                Rank = rank++
            });
        }
        return finalResults;
    }



    private bool IsCostProfileMatch(CostProfile costProfile, ProviderSize providerSize)
    {
        switch (costProfile)
        {
            case CostProfile.Low:
                return providerSize == ProviderSize.VerySmall || providerSize == ProviderSize.Small;

            case CostProfile.Medium:
                return providerSize == ProviderSize.Small || providerSize == ProviderSize.SME;

            case CostProfile.High:
                return providerSize == ProviderSize.SME || providerSize == ProviderSize.Big;

            default:
                return false;
        }
    }
}