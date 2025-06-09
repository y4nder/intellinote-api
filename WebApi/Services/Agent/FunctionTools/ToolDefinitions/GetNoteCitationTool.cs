#pragma warning disable OPENAI001
using System.Text.Json;
using OpenAI.Assistants;
using WebApi.Data.Entities;
using WebApi.Repositories;
using WebApi.Repositories.Note;
using WebApi.Services.Agent.Dtos;
using WebApi.Services.Hubs;
using WebApi.Services.Parsers;

namespace WebApi.Services.Agent.FunctionTools.ToolDefinitions;

public class GetNoteCitationTool : IAgentTool
{
    private readonly INoteRepository _noteRepository;
    private readonly BlockNoteParserService _blockNoteParserService;
    private readonly UserContext<User, string> _userContext;
    private readonly NoteHubService _noteHubService;

    public GetNoteCitationTool(INoteRepository noteRepository, BlockNoteParserService blockNoteParserService,
        UserContext<User, string> userContext, NoteHubService noteHubService)
    {
        _noteRepository = noteRepository;
        _blockNoteParserService = blockNoteParserService;
        _userContext = userContext;
        _noteHubService = noteHubService;
    }

    public string FunctionName => nameof(GetNoteCitationTool);
    public async Task<ToolOutput> ProcessAsync(RequiredAction action)
    {
        using JsonDocument argumentsJson = JsonDocument.Parse(action.FunctionArguments);
        bool hasNoteIdArgument = argumentsJson.RootElement.TryGetProperty("noteId", out JsonElement noteId);
        bool hasTextToFindArgument = argumentsJson.RootElement.TryGetProperty("textToFind", out JsonElement textToFind);
        
        if(!hasNoteIdArgument || !hasTextToFindArgument)
            throw new ArgumentException("Missing noteId or textToFind argument.");
        
        var noteIdValue = noteId.ToString();
        var noteIdGuid = Guid.Parse(noteIdValue);
        
        var textToFindValue = textToFind.ToString();
        
        var noteCitation = await GetNoteCitationFunction(textToFindValue, noteIdGuid);

        if (noteCitation == null)
        {
            return new ToolOutput
            {
                ToolCallId = action.ToolCallId,
                Output = "no citation found."
            };
        }
        
        var stringNoteCitation = JsonSerializer.Serialize(noteCitation);

        return new ToolOutput
        {
            ToolCallId = action.ToolCallId,
            Output = stringNoteCitation
        };
    }

    private async Task<PromptContracts.NoteCitation?> GetNoteCitationFunction(string textToFind, Guid noteId)
    {
        var noteDto = await _noteRepository.FindNoteWithProjection(noteId);
        if (noteDto == null) throw new ArgumentException("Note not found.");

        await _noteHubService.NotifyAgentStep(
            noteDto.Author.Id,
            "Searching for citation in " + noteDto.Title + "..." 
        );
        
        var blockSnippet = _blockNoteParserService.ExtractSnippet(textToFind, noteDto!);
        
        if (blockSnippet == null)
        {
            await _noteHubService.NotifyAgentStep(
                noteDto.Author.Id,
                "Could not find citation"
            );  
            return null;
        };
        
        return new PromptContracts.NoteCitation
        {
            NoteId = noteDto.Id,
            SnippetId = blockSnippet!.Id,
            Text = blockSnippet.Text
        };
    }
}