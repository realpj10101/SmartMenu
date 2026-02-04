using api.DTOs;
using api.Interfaces;
using api.Models;
using api.Settings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace api.Services;

public class MenuRecommendationService : IMenuRecommendationService
{
    private const string EmbedModelName = "nomic-embed-text";
    
    private readonly IMongoCollection<MenuItem> _collectionItems;
    private readonly IOllamaEmbedClient _ollamaEmbedClient;

    public MenuRecommendationService(
        IMongoClient client,
        IMyMongoDbSettings dbSettings,
        IOllamaEmbedClient ollamaEmbedClient)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collectionItems = database.GetCollection<MenuItem>("menu_items");
        _ollamaEmbedClient = ollamaEmbedClient;
    }
    
    public async Task<MenuRecommendResponse> GetTopCandidateAsync(MenuRecommendRequest request, CancellationToken cancellationToken)
    {
        string query = (request.Query ?? string.Empty).Trim();
        
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Query cannot be empty.");

        int topN = request.TopN <= 0 ? 20 : Math.Min(request.TopN, 50);

        float[] q = await _ollamaEmbedClient.EmbedAsync(EmbedModelName, query, cancellationToken);
        
        if (q.Length == 0)
            throw new ArgumentException("Query embedding is empty.");

        FilterDefinition<MenuItem> filter = Builders<MenuItem>.Filter.Ne(doc => doc.Embedding, null);

        using var cursor = await _collectionItems
            .Find(filter)
            .Project(item => new MenuItemCandidate
            {
                Id = item.Id,
                CategoryNameFa = item.CategoryNameFa,
                PersianName = item.PersianName,
                EnglishName = item.EnglishName,
                Ingredients = item.Ingredients,
                ImageUrl = item.ImageUrl,
                PriceValue = item.PriceValue,
                Sizes = item.Sizes,
                Embedding = item.Embedding!,
            })
            .ToCursorAsync(cancellationToken);
        
        var top = new List<ScoredCandidate>(capacity: topN);

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var item in cursor.Current)
            {
                if (item.Embedding is null || item.Embedding.Length == 0)
                    continue;

                double score = CosineSimilarity(q, item.Embedding);
                
                TryAddTopN(top, new ScoredCandidate(item, score), topN);
            }
        }
        
        top.Sort((a, b) => b.Score.CompareTo(a.Score));
        
        var candidates = top.Select(item => new MenuCandidateDto(
            Id: item.Item.Id.ToString(),
            CategoryNameFa: item.Item.CategoryNameFa,
            PersianName: item.Item.PersianName,
            EnglishName: item.Item.EnglishName,
            Ingredients: item.Item.Ingredients,
            ImageUrl: item.Item.ImageUrl,
            PriceValue: item.Item.PriceValue,
            Sizes: item.Item.Sizes,
            Score: item.Score
        )).ToList();

        return new MenuRecommendResponse(query, topN, candidates);
    }

    private static void TryAddTopN(List<ScoredCandidate> top, ScoredCandidate cand, int topN)
    {
        if (top.Count < topN)
        {
            top.Add(cand);
            return;
        }

        int minIdx = 0;
        double minScore = top[0].Score;
        for (int i = 1; i < top.Count; i++)
        {
            if (top[i].Score < minScore)
            {
                minScore = top[i].Score;
                minIdx = i;
            }
        }

        if (cand.Score > minScore)
            top[minIdx] = cand;
    }

    private static double CosineSimilarity(float[] a, float[] b)
    {
        int len = Math.Min(a.Length, b.Length);
        if (len == 0) return 0;

        double dot = 0;
        double normA = 0;
        double normB = 0;

        for (int i = 0; i < len; i++)
        {
            double x = a[i];
            double y = b[i];
            dot += x * y;
            normA += x * x;
            normB += y * y;
        }
        
        double denom = Math.Sqrt(normA) * Math.Sqrt(normB);
        
        if (denom == 0) return 0;
        
        return dot / denom;
    }
    
    private sealed class MenuItemCandidate
    {
        public ObjectId Id { get; set; }
        public string CategoryNameFa { get; set; } = "";
        public string PersianName { get; set; } = "";
        public string EnglishName { get; set; } = "";
        public string Ingredients { get; set; } = "";
        public string? ImageUrl { get; set; }
        public int? PriceValue { get; set; }

        public List<MenuSizePrice> Sizes { get; set; }
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }

    private sealed record ScoredCandidate(MenuItemCandidate Item, double Score);
}