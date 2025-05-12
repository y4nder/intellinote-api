#pragma warning disable OPENAI001

using System.Text.Json;
using OpenAI.Assistants;
using WebApi.Data.Entities;
using WebApi.Repositories;

namespace WebApi.Services.Agent.FunctionTools.ToolDefinitions;

public class GetUserFolderTool : IAgentTool
{
    public string FunctionName => nameof(GetUserFolderTool);
    private readonly FolderRepository _folderRepository;

    public GetUserFolderTool(FolderRepository folderRepository)
    {
        _folderRepository = folderRepository;
    }

    public async Task<ToolOutput> ProcessAsync(RequiredAction action)
    {
        using JsonDocument argumentsJson = JsonDocument.Parse(action.FunctionArguments);
        bool hasNoteIdArgument = argumentsJson.RootElement.TryGetProperty("folderId", out JsonElement folderId);
        
        if (!hasNoteIdArgument)
            throw new ArgumentException("Missing folderId argument.");
        
        var folderIdValue = folderId.ToString();
        var folderIdGuid = Guid.Parse(folderIdValue);
        
        var folder = await GetUserFolderFunction(folderIdGuid);
        
        var folderString = JsonSerializer.Serialize(folder);

        return new ToolOutput
        {
            ToolCallId = action.ToolCallId,
            Output = folderString,
        };
    }

    private async Task<FolderWithDetailsDtoMinimal> GetUserFolderFunction(Guid folderId)
    {
        var folder = await _folderRepository.GetFolderWithDetailsMinimalAsync(folderId);
        if(folder == null) throw new ArgumentException("Folder not found.");
        return folder;
    }
}