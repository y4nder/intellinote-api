#pragma warning disable OPENAI001
using System.Text.Json;
using OpenAI.Assistants;
using WebApi.Data.Entities;
using WebApi.Repositories;
using WebApi.Repositories.Folder;
using WebApi.Services.Http;
using WebApi.Services.Hubs;

namespace WebApi.Services.Agent.FunctionTools.ToolDefinitions;

public class GetTopFoldersTool : IAgentTool
{
    public GetTopFoldersTool(IFolderRepository folderRepository, EmbeddingService embeddingService, UserContext<User, string> userContext, NoteHubService noteHubService)
    {
        _folderRepository = folderRepository;
        _embeddingService = embeddingService;
        _userContext = userContext;
        _noteHubService = noteHubService;
    }

    public string FunctionName => nameof(GetTopFoldersTool);
    private readonly IFolderRepository _folderRepository;
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

        string outputFoldersString = "";
        
        if (string.IsNullOrWhiteSpace(searchTermValue))
        {
            await _noteHubService.NotifyAgentStep(_userContext.Id(), "Searching on all folders...");
            var allFolders = await ListFoldersFunction();
            outputFoldersString = JsonSerializer.Serialize(allFolders);             
        }
        else
        {
            await _noteHubService.NotifyAgentStep(_userContext.Id(), "Searching your folders for " + searchTermValue + "...");
            var topFolders = await GetTopFoldersFunction(searchTermValue, topValue);
            outputFoldersString = JsonSerializer.Serialize(topFolders);             
        }

        return new ToolOutput
        {
            ToolCallId = action.ToolCallId,
            Output = outputFoldersString
        };
    }

    private async Task<List<FolderWithDetailsDtoMinimal>> GetTopFoldersFunction(string searchTerm, int top = 5)
    {
        var searchVector = await _embeddingService.GenerateEmbeddings(searchTerm);
        return await _folderRepository.GetTopFoldersForAgent(_userContext.Id(), searchVector, top:top);
    } 
    
    private async Task<List<FolderWithoutDetailsDto>> ListFoldersFunction()
    {
        return await _folderRepository.GetFoldersWithoutDetailsMinimalAsync(_userContext.Id(),100);
    } 
}