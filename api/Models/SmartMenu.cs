using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Models;

public class MenuCategory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; init; }

    public string NameFa { get; init; } = string.Empty;

    public string Slug { get; init; } = string.Empty;

    public int SortOrder { get; init; }   
}

public class MenuItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; init; }

    public ObjectId CategoryId { get; init; }

    public string CategorySlug { get; init; } = string.Empty;
    public string CategoryNameFa { get; init; } = string.Empty;

    public string PersianName { get; init; } = string.Empty;
    public string EnglishName { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public string Ingredients { get; init; } = string.Empty;

    public List<MenuSizePrice> Sizes { get; init; } = new();

    public string? Price { get; init; } = string.Empty;
    public int? PriceValue { get; init; }
    
    public float[]? Embedding { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MenuSizePrice
{
    public string Size { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public int PriceValue { get; set; }
}