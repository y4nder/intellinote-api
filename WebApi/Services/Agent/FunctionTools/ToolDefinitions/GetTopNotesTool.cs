#pragma warning disable OPENAI001
using System.Text.Json;
using OpenAI.Assistants;
using WebApi.Data.Entities;
using WebApi.Repositories.Note;
using WebApi.Services.Hubs;

namespace WebApi.Services.Agent.FunctionTools.ToolDefinitions;

public class GetTopNotesTool : IAgentTool
{
    public GetTopNotesTool(INoteRepository noteRepository, EmbeddingService embeddingService, UserContext<User, string> userContext, NoteHubService noteHubService)
    {
        _noteRepository = noteRepository;
        _embeddingService = embeddingService;
        _userContext = userContext;
        _noteHubService = noteHubService;
    }

    public string FunctionName => nameof(GetTopNotesTool);
    
    private readonly INoteRepository _noteRepository;
    private readonly EmbeddingService _embeddingService;
    private readonly UserContext<User, string> _userContext;
    private readonly NoteHubService _noteHubService;
    public async Task<ToolOutput> ProcessAsync(RequiredAction action)
    {
        using JsonDocument argumentsJson = JsonDocument.Parse(action.FunctionArguments);
        bool hasSearchTermArgument = argumentsJson.RootElement.TryGetProperty("searchTerm", out JsonElement searchTerm);
        bool hasTopArgument = argumentsJson.RootElement.TryGetProperty("top", out JsonElement top);
        
        if(!hasSearchTermArgument)
            throw new ArgumentException("Missing searchTerm argument.");
        
        int topValue = 5;

        if (hasTopArgument)
        {
            topValue = top.GetInt32();
        }
        
        var searchTermValue = searchTerm.ToString();

        var topNotes = await GetTopNotesFunction(searchTermValue, topValue);
        
        var topNotesString = JsonSerializer.Serialize(topNotes);

        return new ToolOutput
        {
            ToolCallId = action.ToolCallId,
            Output = topNotesString,
        };
    }

    private async Task<List<NoteDtoVeryMinimal>> GetTopNotesFunction(string searchTerm, int top = 5)
    {
        await _noteHubService.NotifyAgentStep(_userContext.Id(), "Searching your notes for " + searchTerm + "...");
        var searchVector = await _embeddingService.GenerateEmbeddings(searchTerm);
        return await _noteRepository.SearchNotesForAgent(_userContext.Id(), searchVector, top: top);
    }
}