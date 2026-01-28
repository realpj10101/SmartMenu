namespace api.Interfaces;

public interface IMenuEmbeddingService
{
    public Task<(int updated, int skipped)> EmbedAllAsync(CancellationToken cancellationToken);
}