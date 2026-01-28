using api.Interfaces;
using api.Repositories;

namespace api.Extensions;

public static class RepositoryServiceExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IMenuRepository, MenuRepository>();

        return services;
    }
}