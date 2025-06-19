using System.Text.RegularExpressions;

namespace WebApi.Services.Parsers;


public static class TextCleaner
{
    public static string Clean(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        string cleaned = Regex.Replace(input, "<.*?>", " ");

        // Convert to lowercase for normalization
        cleaned = cleaned.ToLowerInvariant();

        // Remove punctuation (preserve letters, digits, and spaces)
        cleaned = Regex.Replace(cleaned, @"[^\p{L}\p{N}\s]", " "); // \p{L} = letters, \p{N} = numbers

        // Normalize whitespace
        cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();

        return cleaned;
    }
}
