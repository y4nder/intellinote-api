using System.Diagnostics;
using Quartz;
using WebApi.Repositories;
using WebApi.Services;
using WebApi.Services.External;
using WebApi.Services.Hubs;

namespace WebApi.Features.Folders.Jobs;

public class GenerateFolderEmbeddings : IJob
{
    private readonly FolderRepository _repository;
    private readonly UnitOfWork _unitOfWork;
    private readonly EmbeddingService _embeddingService;
    private readonly NoteHubService _noteHubService;
    private readonly FolderMetaDataService _folderMetaDataService;
    public const String Name = nameof(GenerateFolderEmbeddings);

    public GenerateFolderEmbeddings(FolderRepository repository,
        UnitOfWork unitOfWork,
        EmbeddingService embeddingService,
        NoteHubService noteHubService,
        FolderMetaDataService folderMetaDataService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _embeddingService = embeddingService;
        _noteHubService = noteHubService;
        _folderMetaDataService = folderMetaDataService;
    }


    public async Task Execute(IJobExecutionContext context)
    {
        var stopWatch = Stopwatch.StartNew();
        var contextMergedJobDataMap = context.MergedJobDataMap;
        var folderId = contextMergedJobDataMap.GetString("folderId");
        var stringAuto = contextMergedJobDataMap.GetString("auto");
        var auto = bool.Parse(stringAuto ?? "false");
        var descriptionToEmbed = contextMergedJobDataMap.GetString("descriptionToEmbed");
        
        if (string.IsNullOrEmpty(folderId)) return;
        if (string.IsNullOrEmpty(descriptionToEmbed)) return;
        
        var parsedId = Guid.Parse(folderId);
        var folder = await _repository.FindByIdAsync(parsedId);
        if(folder == null) return;
        
        var folderEmbeddingsVector = await _embeddingService.GenerateEmbeddings(descriptionToEmbed);
        
        folder.Embedding = folderEmbeddingsVector;
        
        var metadata = await HandleGeneration(descriptionToEmbed);
        folder.Description = metadata.Description;
        
        if (auto) //usually from folder creation
        {
            folder.Name = metadata.Title;
        }
        
        await _unitOfWork.Commit(CancellationToken.None);
        stopWatch.Stop();
        if (auto)
        {
            await _noteHubService.NotifyFolderCreationDone(folder, stopWatch.ElapsedMilliseconds);
        }
        else
        {
            await _noteHubService.NotifyFolderUpdateDone(folder, stopWatch.ElapsedMilliseconds);
        }
            
    }

    private async Task<FolderMetadataDto> HandleGeneration(string text)
    {
        var metadataDto = await _folderMetaDataService.GenerateFolderMetadata(text);
        return metadataDto;
    }
}