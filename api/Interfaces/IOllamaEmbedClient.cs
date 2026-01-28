namespace api.Interfaces;

public interface IOllamaEmbedClient
{
    public Task<float[]> EmbedAsync(string model, string input, CancellationToken cancellationToken);
}