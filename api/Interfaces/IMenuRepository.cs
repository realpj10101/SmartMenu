using api.Models;

namespace api.Interfaces;

public interface IMenuRepository
{
    public Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync(CancellationToken cancellationToken);
}