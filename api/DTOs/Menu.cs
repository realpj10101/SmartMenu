namespace api.DTOs;

public record MenuRecommendRequest(
    string Query,
    int TopN = 20
);

public record MenuCandidateDto(
    string Id,
    string CategoryNameFa,
    string PersianName,
    string EnglishName,
    string Ingredients,
    string? ImageUrl,
    int? PriceValue,
    double Score
);

public record MenuRecommendResponse(
    string Query,
    int TopN,
    List<MenuCandidateDto> Candidates
);

