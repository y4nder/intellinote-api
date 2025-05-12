#pragma warning disable OPENAI001
using System.ClientModel;
using Microsoft.Extensions.Options;
using OpenAI.Assistants;
using WebApi.Extensions;
using WebApi.Services.Agent.Dtos;
using Newtonsoft.Json;

namespace WebApi.Services.Agent.Agents;


public class Nora 
    : IChatAgent<PromptContracts.PromptRequestDto, PromptContracts.PromptResponseDto>
{
    public string AgentName => nameof(Nora);
    private readonly AssistantClient _assistantClient;
    private readonly ToolRouter _toolRouter;
    private readonly string _assistantId;

    
    public Nora(AssistantClient assistantClient, ToolRouter toolRouter, IOptions<OpenAiSettings> options)
    {
        _assistantClient = assistantClient;
        _toolRouter = toolRouter;
        _assistantId = options.Value.AssistantId;
    }
    
    public async Task<PromptContracts.PromptResponseDto> ProcessPromptAsync(PromptContracts.PromptRequestDto prompt)
    {
        ClientResult<ThreadRun> threadRun;
        
        if (!string.IsNullOrEmpty(prompt.ThreadId))
        {
            threadRun = await _assistantClient.CreateRunAsync(prompt.ThreadId, _assistantId,
                new RunCreationOptions
                {
                    AdditionalMessages = { prompt.PromptMessage },
                });
        }
        else
        {
            string initialMessage = "The provided id contexts: ";
            
            // TODO add folder context prompt feature
            var context = prompt.PromptContext;
            if (context.FolderId is null && context.NoteId is null)
            {
                throw new ArgumentException("Either folderId or noteId must be provided.");
            }
            
            if (context.FolderId is not null && context.NoteId is not null)
            {
                throw new ArgumentException("Only one of folderId or noteId can be provided.");
            }

            if (context.FolderId is not null)
            {
                initialMessage += $"(folderId: {context.FolderId})";
            }

            if (context.NoteId is not null)
            {
                initialMessage += $"(noteId: {context.NoteId})";
            }
            
            ThreadInitializationMessage initializationMessage = new(MessageRole.Assistant, [initialMessage]);
            
            threadRun = await _assistantClient.CreateThreadAndRunAsync(_assistantId, new ThreadCreationOptions
            {
                InitialMessages =
                {
                    initializationMessage,
                    prompt.PromptMessage
                }
            });
        }
        
        while (!threadRun.Value.Status.IsTerminal)
        {
            await ProcessRequiredActions(_assistantClient, threadRun);
            threadRun = await _assistantClient.GetRunAsync(threadRun.Value.ThreadId, threadRun.Value.Id);
        }
        var messageItem  = _assistantClient.GetMessages(threadRun.Value.ThreadId).First();
        var content = messageItem.Content.First().Text;
        var result = JsonConvert.DeserializeObject<PromptContracts.PromptResponseDto>(content)!;
        result.ThreadId = threadRun.Value.ThreadId;
        return result;
    }

    public async Task ProcessRequiredActions(
        AssistantClient assistantClient, 
        ClientResult<ThreadRun> threadRun)
    {
        if (threadRun.Value.Status != RunStatus.RequiresAction)
            return;
        
        var outputs = new List<ToolOutput>();
        
        foreach (var action in threadRun.Value.RequiredActions)
        {
            var result = await _toolRouter.RouteToolAsync(action);
            outputs.Add(result);
        }
        
        await assistantClient.SubmitToolOutputsToRunAsync(
            threadRun.Value.ThreadId,
            threadRun.Value.Id,
            outputs
        );
    }
}