using api.Clients;
using api.Interfaces;
using api.Repositories;
using api.Services;

namespace api.Extensions;

public static class RepositoryServiceExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IMenuEmbeddingService, MenuEmbeddingService>();
        services.AddScoped<IOllamaEmbedClient, OllamaEmbedClient>();
        services.AddScoped<IMenuRecommendationService, MenuRecommendationService>();
        services.AddScoped<IMenuRecommendExplainService, MenuRecommendExplainService>();
        services.AddScoped<IChatClient, OllamaChatClient>();

        return services;
    }
}