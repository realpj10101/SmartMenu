using System.Text.Json;
using api.DTOs;
using api.Interfaces;

namespace api.Services;

public class MenuRecommendExplainService : IMenuRecommendExplainService
{
    private const string LlmModel = "qwen2.5:7b-instruct";

    private readonly IMenuRecommendationService _recommendationService;
    private readonly IChatClient _chat;

    public MenuRecommendExplainService(
        IMenuRecommendationService recommendation,
        IChatClient chat)
    {
        _recommendationService = recommendation;
        _chat = chat;
    }

    public async Task<MenuRecommendExplainResponse> RecommendAndExplainAsync(MenuRecommendExplainRequest request,
        CancellationToken cancellationToken)
    {
        string query = (request.Query ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Query cannot be empty.");

        int topN = request.TopN <= 0 ? 5 : Math.Min(request.TopN, 10);
        int topK = request.TopK <= 0 ? 3 : Math.Min(request.TopK, 5);
        
        MenuRecommendResponse stage1 = await _recommendationService.GetTopCandidateAsync(
            new MenuRecommendRequest(query, topN), cancellationToken
        );
        
        if (stage1.Candidates.Count == 0)
        {
            return new MenuRecommendExplainResponse(
                query,
                topN,
                topK,
                "چیزی که دقیقاً با این توصیف بخوره پیدا نکردم. اگر دوست داری، می‌تونی کمی توضیحت رو عوض کنی یا ساده‌تر بگی چی می‌خوای.",
                new()
            );
        }

        List<MenuCandidateDto> llmCandidates = stage1.Candidates
            .Take(topN)
            .ToList();

        string systemPrompt = """
                              تو یک باریستا/مشاور انتخاب منوی کافه هستی.
                              فقط فارسیِ روان و طبیعی بنویس.
                              فقط از بین آیتم‌هایی که من می‌دهم انتخاب کن و هیچ آیتم جدیدی نساز.
                              خروجی باید فقط JSON معتبر باشد؛ هیچ متن اضافه‌ای قبل یا بعد از JSON ننویس.
                              """;

        string userPrompt = BuildUserPrompt(query, llmCandidates, topK);

        string raw = await _chat.ChatAsync(
            model: "qwen2.5:7b-instruct",
            system: systemPrompt,
            user: userPrompt,
            cancellationToken: cancellationToken
        );

        string json = ExtractFirstJsonObject(raw);

        var (summary, picks) = ParseExplainJson(json);

        HashSet<string> allowedIds = stage1.Candidates
            .Select(c => c.Id)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        picks = picks
            .Where(p => allowedIds.Contains(p.Id))
            .Take(topK)
            .ToList();

        if (string.IsNullOrWhiteSpace(summary))
        {
            summary = "بر اساس چیزی که گفتی، این گزینه‌ها از نظر طعم و حس کلی به حالت نزدیک‌تر بودن.";
        }
        
        return new MenuRecommendExplainResponse(
            query,
            topN,
            topK,
            summary,
            picks
        );
    }

    public async Task<MenuRecommendTalkResponse> RecommendAndTalkAsync(MenuRecommendExplainRequest request, CancellationToken cancellationToken)
    {
        string query = (request.Query ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Query cannot be empty.");
        
        int topN = request.TopN <= 0 ? 5 : Math.Min(request.TopN, 10);
        
        MenuRecommendResponse stage1 = await _recommendationService.GetTopCandidateAsync(new MenuRecommendRequest(query, topN), cancellationToken);

        if (stage1.Candidates.Count == 0)
        {
            return new MenuRecommendTalkResponse(
                query, topN,
                "می‌فهممت. چیزی که دقیقاً بخوره به این توصیف پیدا نکردم. اگه دوست داری بگو شیرین‌تر می‌خوای یا تلخ‌تر، سرد یا گرم؟",
                new()
            );
        }

        var brief = stage1.Candidates.Select(c => new
        {
            c.PersianName,
            c.CategoryNameFa,
            Price = c.PriceValue,
            Sizes = (c.PriceValue is null || c.PriceValue == 0) ? c.Sizes : null
        });
        
        string system = """
                        تو یک باریستا/مشاور انتخاب منوی کافه هستی.
                        فقط فارسی روان و صمیمی بنویس.
                        متن کوتاه باشد (حداکثر ۳-۴ جمله).
                        اسم چند گزینه را هم داخل متن بیاور.
                        """;

        string user = $"""
                       نیاز کاربر: "{query}"

                       گزینه‌هایی که سیستم پیشنهاد داده:
                       {brief}

                       یک پیام کوتاه و صمیمانه بنویس که:
                       - همدلی کند
                       - بگو چرا این گزینه‌ها مناسب‌اند
                       - آخرش دعوت کند اگر دقیق‌تر بگوید (شیرین/تلخ، سرد/گرم)
                       """;

        string message = await _chat.ChatAsync(LlmModel, system, user, cancellationToken);
        if (string.IsNullOrWhiteSpace(message))
            message = "می‌فهممت. این چندتا گزینه به چیزی که گفتی نزدیک‌ترن و احتمالاً حالت رو بهتر می‌کنن.";

        return new MenuRecommendTalkResponse(
            query,
            topN,
            message.Trim(),
            stage1.Candidates
        );
    }

    private static string BuildUserPrompt(string query, List<MenuCandidateDto> candidates, int topK)
    {
        var compact = candidates.Select(c => new
        {
            c.Id,
            c.CategoryNameFa,
            c.PersianName,
            c.EnglishName,
            c.Ingredients,
            c.PriceValue,
            c.Score
        });

        string candidatesJson = JsonSerializer.Serialize(compact);

        return $$"""
                 نیاز کاربر: "{{query}}"

                 وظیفه:
                 - فقط از بین همین کاندیدها دقیقاً {{topK}} مورد انتخاب کن.
                 - برای هر انتخاب، یک دلیل صمیمانه و کوتاه فارسی بنویس (۱ تا ۲ جمله).
                 - علاوه بر آن، یک جمع‌بندی صمیمانه فارسی هم بده که چرا این گزینه‌ها مناسب‌اند.
                 - فقط JSON معتبر خروجی بده (هیچ متن اضافه‌ای قبل یا بعدش ننویس).

                 کاندیدها (JSON):
                 {{candidatesJson}}

                 فرمت خروجی (فقط JSON):
                 {
                   "summaryFa": "یک جمع‌بندی کوتاه و صمیمانه...",
                   "picks": [
                     { "id": "ID", "reasonFa": "..." }
                   ]
                 }
                 """;
    }

    private static (string summary, List<MenuPickDto> picks) ParseExplainJson(string json)
    {
        JsonDocument doc = JsonDocument.Parse(json);

        string summary = "";
        if (doc.RootElement.TryGetProperty("summaryFa", out var sEl))
            summary = (sEl.GetString() ?? "").Trim();

        List<MenuPickDto> picks = [];

        if (doc.RootElement.TryGetProperty("picks", out var pickEl) && pickEl.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement pick in pickEl.EnumerateArray())
            {
                string id = (pick.GetProperty("id").GetString() ?? "").Trim();
                string reason = (pick.GetProperty("reasonFa").GetString() ?? "").Trim();
                if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(reason))
                    picks.Add(new MenuPickDto(id, reason));
            }
        }

        return (summary, picks);
    }

    private static string ExtractFirstJsonObject(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new Exception("LLM returned empty response.");

        int start = text.IndexOf('{');
        if (start < 0)
            throw new Exception("LLM response does not contain JSON object.");

        int depth = 0;
        bool inString = false;

        for (int i = start; i < text.Length; i++)
        {
            char ch = text[i];

            if (ch == '"' && (i == 0 || text[i - 1] != '\\'))
                inString = !inString;


            if (inString) continue;

            if (ch == '{') depth++;
            else if (ch == '}') depth--;

            if (depth == 0)
                return text.Substring(start, i - start + 1);
        }

        throw new Exception("Could not extract complete JSON object from LLM response.");
    }
}