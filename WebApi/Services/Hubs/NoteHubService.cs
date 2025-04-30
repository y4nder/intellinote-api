using Microsoft.AspNetCore.SignalR;
using WebApi.Data.Entities;

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
}