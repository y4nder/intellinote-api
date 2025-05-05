using Microsoft.AspNetCore.SignalR;
using WebApi.Services.External;

namespace WebApi.Services.Hubs;

public interface INoteUpdateClient
{
    Task NotifyStandard(NotificationStandardDto message);
    Task BroadcastConnection(string message);
    Task BroadcastMessage(BroadcastMessageDto message);
    
    Task NotifyEmbeddingDone(EmbeddingDoneDto message);
    
    Task NotifyGenerationDone(GenerationDoneDto message);
    
    Task NotifyGenerationFailed(GenerationFailedDto message);
    
    Task NotifyFolderCreationDone(FolderCreationDoneDto message);

    Task NotifyFolderUpdateDone(FolderUpdateDoneDto message);
    
    Task ManualDevNotify(string message);   
}

public class NotificationStandardDto
{
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
}

public class BroadcastMessageDto
{
    public string Message { get; set; } = null!;
    public DateTime DateTime { get; set; } = DateTime.UtcNow;
}

public class EmbeddingDoneDto
{
    public string Message { get; set; } = null!;
    public DateTime DateTime { get; set; } = DateTime.UtcNow;   
    public long MilleSeconds { get; set; }   
}   

public class GenerationDoneDto
{
    public string Name { get; set; } = null!;
    public string Id { get; set; } = null!;
    public GeneratedResponseDto Response { get; set; } = null!;
    public DateTime DateTime { get; set; } = DateTime.UtcNow;
    public long MilleSeconds { get; set; }
}

public class GenerationFailedDto
{
    public string Message { get; set; } = null!;
    public DateTime DateTime { get; set; } = DateTime.UtcNow;
    public long MilleSeconds { get; set; }
}

public class FolderCreationDoneDto
{
    public string Message { get; set; } = null!;
    public Guid FolderId { get; set; }
    DateTime DateTime { get; set; } = DateTime.UtcNow;
    public long MilliSeconds { get; set; }
}

public class FolderUpdateDoneDto
{
    public string Message { get; set; } = null!;
    public Guid FolderId { get; set; }
    DateTime DateTime { get; set; } = DateTime.UtcNow;
    public long MilliSeconds { get; set; }
}

public class NoteHub : Hub<INoteUpdateClient>
{
    private readonly ILogger<NoteHub> _logger;

    public NoteHub(ILogger<NoteHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            _logger.LogInformation("Connected: {ConnectionId}", Context.ConnectionId);
            _logger.LogInformation("Connected user: {UserIdentifier}", Context.UserIdentifier);
            await Clients.Caller.BroadcastMessage(new BroadcastMessageDto { Message = $"Connected: {Context.ConnectionId}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnConnectedAsync");
        }

        await base.OnConnectedAsync();
    }

    public void EmbeddingAcknowledged(string message)
    {
        _logger.LogInformation($"[EMB_ACK]: {message}");
    }
    
    
}   