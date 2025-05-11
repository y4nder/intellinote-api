namespace WebApi.Services.Stemmer;

public class QueryMatcher
{
    public static (bool matches, int score) QueryMatchScore(string text, string query)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(query))
            return (false, 0);

        string[] textWords = ProcessText(text);
        string[] queryWords = ProcessText(query);

        if (queryWords.Length == 0)
            return (false, 0);

        int score = 0;
        
        // Check for consecutive matches
        for (int i = 0; i <= textWords.Length - queryWords.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < queryWords.Length; j++)
            {
                if (textWords[i + j] != queryWords[j])
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                score += 100;
            }
        }

        // Count individual word matches
        int wordMatches = queryWords.Count(qw => textWords.Contains(qw));
        score += wordMatches * 10;

        return (wordMatches > 0, score);
    }
    
    public static bool IsQueryInText(string text, string query)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(query))
            return false;

        // Normalize and stem both strings
        string[] textWords = ProcessText(text);
        string[] queryWords = ProcessText(query);

        if (queryWords.Length == 0)
            return false;

        // Check if query words appear consecutively in text
        for (int i = 0; i <= textWords.Length - queryWords.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < queryWords.Length; j++)
            {
                if (textWords[i + j] != queryWords[j])
                {
                    match = false;
                    break;
                }
            }
            if (match)
                return true;
        }
        
        return false;
        return queryWords.Any(qw => textWords.Contains(qw));
    }

    private static string[] ProcessText(string input)
    {
        return input.ToLower()
                    .Split([' ', '.', ',', '!', '?'], StringSplitOptions.RemoveEmptyEntries)
                    .Select(PorterStemmer.Stem)
                    .ToArray();
    }
}

// Implementation of Porter Stemmer algorithm
public static class PorterStemmer
{
    // This is a simplified version of the Porter Stemmer
    public static string Stem(string word)
    {
        if (string.IsNullOrEmpty(word))
            return word;

        // Step 1a: Handle plurals and -ed, -ing
        if (word.EndsWith("sses"))
            word = word.Substring(0, word.Length - 2);
        else if (word.EndsWith("ies"))
            word = word.Substring(0, word.Length - 2);
        else if (word.EndsWith("ss"))
            word = word.Substring(0, word.Length);
        else if (word.EndsWith("s"))
            word = word.Substring(0, word.Length - 1);

        // Step 1b: Handle other endings
        if (word.EndsWith("eed"))
        {
            if (Measure(word.Substring(0, word.Length - 3)) > 0)
                word = word.Substring(0, word.Length - 1);
        }
        else if ((word.EndsWith("ed") && ContainsVowel(word.Substring(0, word.Length - 2))) ||
                 (word.EndsWith("ing") && ContainsVowel(word.Substring(0, word.Length - 3))))
        {
            word = word.EndsWith("ed") ? word.Substring(0, word.Length - 2) : word.Substring(0, word.Length - 3);
            
            // Additional rules for step 1b
            if (word.EndsWith("at") || word.EndsWith("bl") || word.EndsWith("iz"))
                word += "e";
            else if (IsDoubleConsonant(word) && !(word.EndsWith("l") || word.EndsWith("s") || word.EndsWith("z")))
                word = word.Substring(0, word.Length - 1);
            else if (Measure(word) == 1 && IsCvc(word))
                word += "e";
        }

        // Step 1c: Handle -y
        if (word.EndsWith("y") && ContainsVowel(word.Substring(0, word.Length - 1)))
            word = word.Substring(0, word.Length - 1) + "i";

        // Step 2: Handle common suffixes
        word = ReplaceSuffix(word, "ational", "ate");
        word = ReplaceSuffix(word, "tional", "tion");
        word = ReplaceSuffix(word, "enci", "ence");
        word = ReplaceSuffix(word, "anci", "ance");
        word = ReplaceSuffix(word, "izer", "ize");
        word = ReplaceSuffix(word, "abli", "able");
        word = ReplaceSuffix(word, "alli", "al");
        word = ReplaceSuffix(word, "entli", "ent");
        word = ReplaceSuffix(word, "eli", "e");
        word = ReplaceSuffix(word, "ousli", "ous");
        word = ReplaceSuffix(word, "ization", "ize");
        word = ReplaceSuffix(word, "ation", "ate");
        word = ReplaceSuffix(word, "ator", "ate");
        word = ReplaceSuffix(word, "alism", "al");
        word = ReplaceSuffix(word, "iveness", "ive");
        word = ReplaceSuffix(word, "fulness", "ful");
        word = ReplaceSuffix(word, "ousness", "ous");
        word = ReplaceSuffix(word, "aliti", "al");
        word = ReplaceSuffix(word, "iviti", "ive");
        word = ReplaceSuffix(word, "biliti", "ble");

        // Step 3: Handle more suffixes
        word = ReplaceSuffix(word, "icate", "ic");
        word = ReplaceSuffix(word, "ative", "");
        word = ReplaceSuffix(word, "alize", "al");
        word = ReplaceSuffix(word, "iciti", "ic");
        word = ReplaceSuffix(word, "ical", "ic");
        word = ReplaceSuffix(word, "ful", "");
        word = ReplaceSuffix(word, "ness", "");

        // Step 4: Handle final suffixes
        if (Measure(word) > 1)
        {
            word = ReplaceSuffix(word, "al", "");
            word = ReplaceSuffix(word, "ance", "");
            word = ReplaceSuffix(word, "ence", "");
            word = ReplaceSuffix(word, "er", "");
            word = ReplaceSuffix(word, "ic", "");
            word = ReplaceSuffix(word, "able", "");
            word = ReplaceSuffix(word, "ible", "");
            word = ReplaceSuffix(word, "ant", "");
            word = ReplaceSuffix(word, "ement", "");
            word = ReplaceSuffix(word, "ment", "");
            word = ReplaceSuffix(word, "ent", "");
            word = ReplaceSuffix(word, "ion", "", c => c == 's' || c == 't');
            word = ReplaceSuffix(word, "ou", "");
            word = ReplaceSuffix(word, "ism", "");
            word = ReplaceSuffix(word, "ate", "");
            word = ReplaceSuffix(word, "iti", "");
            word = ReplaceSuffix(word, "ous", "");
            word = ReplaceSuffix(word, "ive", "");
            word = ReplaceSuffix(word, "ize", "");
        }

        // Step 5a: Remove final -e if measure > 1
        if (Measure(word) > 1 && word.EndsWith("e"))
            word = word.Substring(0, word.Length - 1);

        // Step 5b: Remove final -l if measure > 1 and Word ends with double l
        if (Measure(word) > 1 && IsDoubleConsonant(word) && word.EndsWith("l"))
            word = word.Substring(0, word.Length - 1);

        return word;
    }

    private static string ReplaceSuffix(string word, string suffix, string replacement, Func<char, bool>? condition = null)
    {
        if (word.EndsWith(suffix))
        {
            string stem = word.Substring(0, word.Length - suffix.Length);
            if (condition == null || condition(stem.LastOrDefault()))
            {
                if (Measure(stem) > 0)
                    return stem + replacement;
            }
        }
        return word;
    }

    private static bool ContainsVowel(string word)
    {
        return word.Any(IsVowel);
    }

    private static bool IsVowel(char c)
    {
        return c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u';
    }

    private static bool IsDoubleConsonant(string word)
    {
        if (word.Length < 2)
            return false;
        return word[^1] == word[^2] && !IsVowel(word[^1]);
    }

    private static bool IsCvc(string word)
    {
        if (word.Length < 3)
            return false;
        return !IsVowel(word[^1]) && 
               IsVowel(word[^2]) && 
               !IsVowel(word[^3]) && 
               word[^1] != 'w' && 
               word[^1] != 'x' && 
               word[^1] != 'y';
    }

    private static int Measure(string stem)
    {
        int count = 0;
        bool vowelSequence = false;

        for (int i = 0; i < stem.Length; i++)
        {
            if (IsVowel(stem[i]))
            {
                if (!vowelSequence)
                {
                    vowelSequence = true;
                    count++;
                }
            }
            else
            {
                vowelSequence = false;
            }
        }

        return count;
    }
}