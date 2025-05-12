#pragma warning disable OPENAI001
using System.Text.Json;
using OpenAI.Assistants;
using WebApi.Repositories;
using WebApi.Services.Agent.Dtos;
using WebApi.Services.Parsers;

namespace WebApi.Services.Agent.FunctionTools.ToolDefinitions;

public class GetNoteCitationTool : IAgentTool
{
    private readonly NoteRepository _noteRepository;
    private readonly BlockNoteParserService _blockNoteParserService;

    public GetNoteCitationTool(NoteRepository noteRepository, BlockNoteParserService blockNoteParserService)
    {
        _noteRepository = noteRepository;
        _blockNoteParserService = blockNoteParserService;
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
        
        var stringNoteCitation = JsonSerializer.Serialize(noteCitation);

        return new ToolOutput
        {
            ToolCallId = action.ToolCallId,
            Output = stringNoteCitation
        };
    }

    private async Task<PromptContracts.NoteCitation> GetNoteCitationFunction(string textToFind, Guid noteId)
    {
        var noteDto = await _noteRepository.FindNoteWithProjection(noteId);
        
        if (noteDto == null) throw new ArgumentException("Note not found.");
        
        var blockSnippet = _blockNoteParserService.ExtractSnippet(textToFind, noteDto!);

        return new PromptContracts.NoteCitation
        {
            Id = blockSnippet!.Id,
            Text = blockSnippet.Text
        };
    }
}