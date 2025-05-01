using Microsoft.AspNetCore.SignalR;
using WebApi.Data.Entities;
using WebApi.Services.External;

namespace WebApi.Services.Hubs;

public class NoteHubService
{
    private readonly IHubContext<NoteHub, INoteUpdateClient> _hubContext;

    public NoteHubService(IHubContext<NoteHub, INoteUpdateClient> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task NotifyEmbeddingDone(Note note, long milleSeconds)
    {
        var message = $"note: {note.Id} embedding done!\nETA: {milleSeconds}ms";
        // todo change to caller
        await _hubContext.Clients.All.NotifyEmbeddingDone(new EmbeddingDoneDto { Message = message, MilleSeconds = milleSeconds });   
    }

    public async Task NotifyGenerationDone(GeneratedResponse response, long milleSeconds)
    {
        // todo change to caller
        await _hubContext.Clients.All.NotifyGenerationDone(new GenerationDoneDto
        {
            Response = new GeneratedResponseDto
            {
                Keywords = response.Keywords.Select(k => k.Keyword).ToList(),
                Topics = response.Topics,
                Summary = response.Summary,
            },
            MilleSeconds = milleSeconds
        });   
    }

    public async Task NotifyFolderCreationDone(Folder folder, long milleSeconds)
    {
        // todo change to caller
        await _hubContext.Clients.All.NotifyFolderCreationDone(new FolderCreationDoneDto
        {
            FolderId = folder.Id,
            Message = $"Folder {folder.Name} created!",
            MilliSeconds = milleSeconds
        });
    }
    
    public async Task NotifyFolderUpdateDone(Folder folder, long milleSeconds)
    {
        // todo change to caller
        await _hubContext.Clients.All.NotifyFolderUpdateDone(new FolderUpdateDoneDto
        {
            FolderId = folder.Id,
            Message = $"Folder {folder.Name} updated!",
            MilliSeconds = milleSeconds
        });
    }
}