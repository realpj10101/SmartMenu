using api.DTOs;

namespace api.Interfaces;

public interface IMenuRecommendationService
{
    public Task<MenuRecommendResponse> GetTopCandidateAsync(MenuRecommendRequest request, CancellationToken cancellationToken);
}