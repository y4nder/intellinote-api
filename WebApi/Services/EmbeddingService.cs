using OpenAI.Embeddings;
using Pgvector;

namespace WebApi.Services;

public class EmbeddingService
{
    private readonly EmbeddingClient _embeddingClient;

    public EmbeddingService(EmbeddingClient embeddingClient)
    {
        _embeddingClient = embeddingClient;
    }
    
    public async Task<Vector> GenerateEmbeddings(string text)
    {
        var embedding = await _embeddingClient.GenerateEmbeddingAsync(text);
        var embeddingArray = embedding.Value.ToFloats().ToArray();
        return new Vector(embeddingArray);
    }
}