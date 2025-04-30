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
        await _hubContext.Clients.All.NotifyGenerationDone(new GenerationDoneDto
        {
            Response = response,
            MilleSeconds = milleSeconds
        });   
    }
}