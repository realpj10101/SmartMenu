using api.Interfaces;
using api.Models;
using api.Settings;
using MongoDB.Driver;

namespace api.Services;

public class MenuEmbeddingService : IMenuEmbeddingService
{
    private const string ModelName = "nomic-embed-text";
    
    private readonly IMongoCollection<MenuItem> _collectionItems;
    private readonly IOllamaEmbedClient _ollamaEmbedClient;

    public MenuEmbeddingService(IMongoClient client, IMyMongoDbSettings dbSettings, IOllamaEmbedClient ollamaEmbedClient)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collectionItems = database.GetCollection<MenuItem>("menu_items");
        
        _ollamaEmbedClient = ollamaEmbedClient;
    }

    public async Task<(int updated, int skipped)> EmbedAllAsync(CancellationToken cancellationToken)
    {
        int updated = 0, skipped = 0;

        using var cursor = await _collectionItems.Find(doc => doc.Embedding == null).ToCursorAsync(cancellationToken);

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (MenuItem item in cursor.Current)
            {
                string text = BuildText(item);
                
                float[] vector = await _ollamaEmbedClient.EmbedAsync(ModelName, text, cancellationToken);
                
                UpdateDefinition<MenuItem> updateDef = Builders<MenuItem>.Update
                    .Set(doc => doc.Embedding, vector);
                
                UpdateResult result = await _collectionItems.UpdateOneAsync(doc => doc.Id == item.Id, updateDef, null, cancellationToken);

                if (result.ModifiedCount == 1)
                    updated++;
                else
                    skipped++;
            }
        }
        
        return (updated, skipped);
    }

    private static string BuildText(MenuItem item)
        => $"{item.CategoryNameFa} | {item.PersianName} | {item.EnglishName} | {item.Ingredients}";
}
