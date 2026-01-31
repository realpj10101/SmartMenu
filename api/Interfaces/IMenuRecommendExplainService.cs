using api.DTOs;

namespace api.Interfaces;

public interface IMenuRecommendExplainService
{
    public Task<MenuRecommendExplainResponse> RecommendAndExplainAsync(MenuRecommendExplainRequest request, CancellationToken cancellationToken);
    public Task<MenuRecommendTalkResponse> RecommendAndTalkAsync(MenuRecommendExplainRequest request, CancellationToken cancellationToken);
}