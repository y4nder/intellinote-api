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
        var message = $"{note.Title} was embedded!";
        await _hubContext.Clients.User(note.UserId).NotifyEmbeddingDone(new EmbeddingDoneDto { Message = message, MilleSeconds = milleSeconds });
        
        await _hubContext.Clients.User(note.UserId).NotifyStandard(
            NotificationStandardDto.NoteEmbeddingDone(note));
    }

    public async Task NotifyGenerationDone(Note note, GeneratedResponse response, long milleSeconds)
    {
        // todo change to caller
        await _hubContext.Clients.User(note.UserId).NotifyGenerationDone(new GenerationDoneDto
        {
            Id = note.Id.ToString(),
            Name = note.Title,
            Response = new GeneratedResponseDto
            {
                Keywords = response.Keywords.Select(k => k.Keyword).ToList(),
                Topics = response.Topics,
                Summary = response.Summary,
            },
            MilleSeconds = milleSeconds
        });   
        
        await _hubContext.Clients.User(note.UserId).NotifyStandard(
            NotificationStandardDto.NoteSummarizationDone(note)
        );
    }

    public async Task NotifyGenerationFailed(string message, long milleSeconds)
    {
        // todo change to caller
        await _hubContext.Clients.All.NotifyGenerationFailed(new GenerationFailedDto
        {
            Message = message,
            MilleSeconds = milleSeconds
        });   
    }

    public async Task NotifyFolderCreationDone(Folder folder, long milleSeconds)
    {
        // todo change to caller
        await _hubContext.Clients.All.NotifyFolderCreationDone(new FolderCreationDoneDto
        {
            Id = folder.Id,
            Message = $"Folder {folder.Name} created!",
            MilliSeconds = milleSeconds
        });
    }
    
    public async Task NotifyFolderUpdateDone(Folder folder, long milleSeconds)
    {
        // todo change to caller
        await _hubContext.Clients.User(folder.UserId).NotifyFolderUpdateDone(new FolderUpdateDoneDto
        {
            Id = folder.Id,
            FolderDescription = folder.Description,
            Message = $"Folder {folder.Name} updated!",
            MilliSeconds = milleSeconds
        });
    }
    
    public async Task ManualDevNotify()
    {
        await _hubContext.Clients.All.ManualDevNotify("notified ka");
    }
}