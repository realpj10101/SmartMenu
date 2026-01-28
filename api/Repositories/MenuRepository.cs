using api.Interfaces;
using api.Models;
using api.Settings;
using MongoDB.Driver;

namespace api.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly IMongoCollection<MenuItem> _menuCollection;

    public MenuRepository(IMongoClient client, IMyMongoDbSettings dbSettings)
    {
        IMongoDatabase dbName = client.GetDatabase(dbSettings.DatabaseName);

        _menuCollection = dbName.GetCollection<MenuItem>("menu_items");
    }
    
    public async Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync(CancellationToken cancellationToken)
    {
        return await _menuCollection.Find(_ => true).ToListAsync(cancellationToken);
    }
}