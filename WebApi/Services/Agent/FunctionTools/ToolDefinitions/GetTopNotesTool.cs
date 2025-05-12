#pragma warning disable OPENAI001
using System.Text.Json;
using OpenAI.Assistants;
using WebApi.Data.Entities;
using WebApi.Repositories;

namespace WebApi.Services.Agent.FunctionTools.ToolDefinitions;

public class GetTopNotesTool : IAgentTool
{
    public GetTopNotesTool(NoteRepository noteRepository, EmbeddingService embeddingService)
    {
        _noteRepository = noteRepository;
        _embeddingService = embeddingService;
    }

    public string FunctionName => nameof(GetTopNotesTool);
    
    private readonly NoteRepository _noteRepository;
    private readonly EmbeddingService _embeddingService;
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
        var searchVector = await _embeddingService.GenerateEmbeddings(searchTerm);
        return await _noteRepository.SearchNotesForAgent(searchVector, top: top);
    }
}