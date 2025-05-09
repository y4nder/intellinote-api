namespace WebApi.Features.Utilities;

public static class KeywordSimilarity
{
    public static double Compute(List<string> a, List<string> b)
    {
        var meaningfulA = a.Where(x => x.Length > 4).ToHashSet();
        var meaningfulB = b.Where(x => x.Length > 4).ToHashSet();

        var intersection = meaningfulA.Intersect(meaningfulB).Count();
        var union = meaningfulA.Union(meaningfulB).Count();

        return union == 0 ? 0.0 : (double)intersection / union;
    }
}