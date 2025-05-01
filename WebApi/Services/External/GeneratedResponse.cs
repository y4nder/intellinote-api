namespace WebApi.Services.External;

public class KeywordResponse
{
    public string Keyword { get; set; } = null!;
    public double Score { get; set; } 
}

public class GeneratedResponse
{
    public List<KeywordResponse> Keywords { get; set; } = new();
    public string Summary { get; set; } = null!;
    public List<string> Topics { get; set; } = new();
}

public class GeneratedResponseDto
{
    public List<string> Keywords { get; set; } = new();
    public string Summary { get; set; } = null!;
    public List<string> Topics { get; set; } = new();
}