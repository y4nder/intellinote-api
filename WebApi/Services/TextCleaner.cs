using System.Text.RegularExpressions;

namespace WebApi.Services;


public static class TextCleaner
{
    public static string Clean(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove HTML tags
        string cleaned = Regex.Replace(input, "<.*?>", " ");

        // Convert to lowercase
        cleaned = cleaned.ToLowerInvariant();

        // Remove punctuation
        cleaned = Regex.Replace(cleaned, @"[^\w\s]", " ");

        // Normalize whitespace
        cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();

        return cleaned;
    }
}
