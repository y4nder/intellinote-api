#pragma warning disable OPENAI001
using System.Text.Json;
using OpenAI.Assistants;
using WebApi.Data.Entities;
using WebApi.Repositories;
using WebApi.Services.Hubs;

namespace WebApi.Services.Agent.FunctionTools.ToolDefinitions;

public class GetUserNoteTool : IAgentTool
{
    public string FunctionName => nameof(GetUserNoteTool);
    
    private readonly NoteRepository _noteRepository;
    private readonly NoteHubService _noteHubService;
    private readonly UserContext<User, string> _userContext;
    
    public GetUserNoteTool(NoteRepository noteRepository,
        NoteHubService noteHubService,
        UserContext<User, string> userContext)
    {
        _noteRepository = noteRepository;
        _noteHubService = noteHubService;
        _userContext = userContext;
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
        var note = await _noteRepository.GetNormalizedNoteContent(noteId);

        if (note is null) return null;
        await _noteHubService.NotifyAgentStep(note.UserId, "Scanning note " + note.Title + "...");
        
        return note.NormalizedContent;
    }
}