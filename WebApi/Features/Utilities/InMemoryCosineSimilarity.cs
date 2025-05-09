namespace WebApi.Features.Utilities;

public static class InMemoryCosineSimilarity
{
    public static float Compute(float[] vectorA, float[] vectorB)
    {
        if(vectorA.Length != vectorB.Length)
            throw new ArgumentException("Vectors must be of the same length");
        
        float dotProduct = 0f;
        float magnitudeA = 0f;
        float magnitudeB = 0f;
        
        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        if (magnitudeA == 0 || magnitudeB == 0)
            return 0f;
        
        return dotProduct / (float)Math.Sqrt(magnitudeA) / (float)Math.Sqrt(magnitudeB);
    }
}