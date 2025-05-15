using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NetTopologySuite.Geometries.Prepared;
using WebApi.Data.Entities;
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
    
    Task NotifyAgentStep(AgentStepDto message);
}

public class AgentStepDto
{
    public string Message { get; set; } = null!;
}

public class NotificationStandardDto
{
    private const string Note = "Note";
    private const string Folder = "Folder";
    
    public string Type { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;

    public static NotificationStandardDto NoteEmbeddingDone(Note note)
    {
        return new NotificationStandardDto
        {
            Type = Note,
            Title = "Embedding done",
            Message = $"{note.Title} was embedded!",
            Id = note.Id.ToString(),
            Name = note.Title
        };
    }
    
    public static NotificationStandardDto NoteSummarizationDone(Note note)
    {
        return new NotificationStandardDto
        {
            Type = Note,
            Title = "Summarization Finished!",
            Message = "Your note has been summarized! Check it out now!",
            Id = note.Id.ToString(),
            Name = note.Title
        };
    }

    public static NotificationStandardDto NoteSummarizationFailed(Note note)
    {
        return new NotificationStandardDto
        {
            Type = Note,
            Title = "Summarization Failed",
            Message = "Failed to Summarize your note",
            Id = note.Id.ToString(),
            Name = note.Title
        };
    }
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
    public string Id { get; set; } = null!; 
    public string Message { get; set; } = null!;
    public DateTime DateTime { get; set; } = DateTime.UtcNow;
    public long MilleSeconds { get; set; }
}

public class FolderCreationDoneDto
{
    public string Message { get; set; } = null!;
    public Guid Id { get; set; }
    DateTime DateTime { get; set; } = DateTime.UtcNow;
    public long MilliSeconds { get; set; }
}

public class FolderUpdateDoneDto
{
    public string Message { get; set; } = null!;
    public Guid Id { get; set; }
    public string FolderDescription { get; set; } = null!;
    DateTime DateTime { get; set; } = DateTime.UtcNow;
    public long MilliSeconds { get; set; }
}

[Authorize]
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
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Connected: {ConnectionId}", Context.ConnectionId);
            _logger.LogInformation("Connected user: {UserIdentifier}", Context.UserIdentifier);
            await Clients.Caller.BroadcastMessage(new BroadcastMessageDto { Message = $"Socket Connected" });
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