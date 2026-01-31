namespace api.DTOs;

public record MenuRecommendExplainRequest(
    string Query,
    int TopN = 20,
    int TopK = 5
);

public record MenuPickDto(
    string Id,
    string ReasonFa
);

public record MenuRecommendExplainResponse(
    string Query,
    int TopN,
    int TopK,
    string AiSummeryFa,
    List<MenuPickDto> Picks
);

public record MenuRecommendTalkResponse(
    string Query,
    int TopN,
    string MessageFa,
    List<MenuCandidateDto> Candidates
);