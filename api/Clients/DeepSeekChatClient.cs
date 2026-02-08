using System.Net.Http.Headers;
using api.DTOs;
using api.Interfaces;
using api.Settings;
using Microsoft.Extensions.Options;

namespace api.Clients;

public class DeepSeekChatClient : IChatClient
{
    private readonly HttpClient _http;
    private readonly DeepSeekSettings _cfg;

    public DeepSeekChatClient(IHttpClientFactory factory, IOptions<DeepSeekSettings> cfg)
    {
        _http = factory.CreateClient("DeepSeekClient");
        _cfg = cfg.Value;
    }

    public async Task<string> ChatAsync(string model, string system, string user, CancellationToken cancellationToken)
    {
        string mod = string.IsNullOrWhiteSpace(model) ? _cfg.Model : model;

        DeepSeekChatRequest req = new(
            Model: mod,
            Messages: new[]
            {
                new DeepSeekMessage("system", system),
                new DeepSeekMessage("user", user),
            },
            Temperature: 0.4,
            MaxTokens: 350
        );

        using HttpRequestMessage httpReq =
            new(HttpMethod.Post, "chat/completions"); // ⬅ بدون اسلش اول

        httpReq.Content = JsonContent.Create(req);
        httpReq.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _cfg.ApiKey);

        using HttpResponseMessage res =
            await _http.SendAsync(httpReq, cancellationToken);

        res.EnsureSuccessStatusCode();

        DeepSeekChatResponse? body =
            await res.Content.ReadFromJsonAsync<DeepSeekChatResponse>(
                cancellationToken: cancellationToken);

        return (body?.Choices?.FirstOrDefault()?.Message?.Content ?? "").Trim();
    }
}