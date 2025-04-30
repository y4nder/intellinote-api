namespace WebApi.Services.External;

public class GeneratedResponseService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeneratedResponseService> _logger;

    public GeneratedResponseService(HttpClient httpClient,
        ILogger<GeneratedResponseService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GeneratedResponse> GetGeneratedResponse(string text)
    {
        try
        {
            text = TextCleaner.Clean(text);
            _logger.LogInformation($"[GET_RESP]: {text}");
            var response = await _httpClient.GetFromJsonAsync<GeneratedResponse>("/api/generate");
            _logger.LogInformation($"[GET_RESP]: {response}");
            return response!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetGeneratedResponse");
            throw; 
        }
    }

    public async Task<string> HealthCheck()
    {
        try
        {
            var response = await _httpClient.GetStringAsync("");
            _logger.LogInformation($"[HEALTH_CHECK]: {response}");
            return response!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HealthCheck");
            return "Internal Server Error";
        }
    }

}