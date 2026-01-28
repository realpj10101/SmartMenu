using api.DTOs;
using api.Interfaces;
using api.Settings;
using Microsoft.Extensions.Options;

namespace api.Clients;

public class OllamaEmbedClient : IOllamaEmbedClient
{
    private readonly HttpClient _http;
    private readonly OllamaSettings _cfg;

    public OllamaEmbedClient(IHttpClientFactory factory, IOptions<OllamaSettings> cfg)
    {
        _http = factory.CreateClient("OllamaClient");
        _cfg = cfg.Value;
    }
    
    public async Task<float[]> EmbedAsync(string model, string input, CancellationToken cancellationToken)
    {
        HttpResponseMessage? res = await _http.PostAsJsonAsync("/api/embed", new
        {
            model,
            input
        }, cancellationToken);
        
        res.EnsureSuccessStatusCode();
        
        OllamaEmbedResponse? body = await res.Content.ReadFromJsonAsync<OllamaEmbedResponse>(cancellationToken: cancellationToken);
        float[]? vec = body?.embeddings?.FirstOrDefault();

        if (vec is null || vec.Length == 0)
            throw new Exception("Ollama returned empty embedding");

        return vec;
    }
}