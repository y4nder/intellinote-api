#pragma warning disable OPENAI001
using System.Text.Json;
using OpenAI.Assistants;
using WebApi.Repositories;
using WebApi.Services.Parsers;

namespace WebApi.Services.Agent.FunctionTools.ToolDefinitions;

public class GetUserNoteTool : IAgentTool
{
    public string FunctionName => nameof(GetUserNoteTool);
    
    private readonly NoteRepository _noteRepository;
    private readonly BlockNoteParserService _blockNoteParserService;
    

    public GetUserNoteTool(NoteRepository noteRepository, BlockNoteParserService blockNoteParserService)
    {
        _noteRepository = noteRepository;
        _blockNoteParserService = blockNoteParserService;
    }

    public async Task<ToolOutput> ProcessAsync(RequiredAction action)
    {
        using JsonDocument argumentsJson = JsonDocument.Parse(action.FunctionArguments);
        bool hasNoteIdArgument = argumentsJson.RootElement.TryGetProperty("noteId", out JsonElement noteId);
        
        if (!hasNoteIdArgument)
            throw new ArgumentException("Missing noteId argument.");
        
        var noteIdValue = noteId.ToString();
        var noteIdGuid = Guid.Parse(noteIdValue);
        
        var noteContent = await GetUserNoteFunction(noteIdGuid);
        
        if(string.IsNullOrWhiteSpace(noteContent)) 
           throw new ArgumentException("Note not found.");

        return new ToolOutput
        {
            ToolCallId = action.ToolCallId,
            Output = noteContent,
        };
    }

    private async Task<string?> GetUserNoteFunction(Guid noteId)
    {
        //todo add caching
        var note = await _noteRepository.GetNormalizedNoteContent(noteId);
        return note;
    }
}