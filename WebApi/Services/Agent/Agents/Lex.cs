#pragma warning disable OPENAI001
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAI.Assistants;
using OpenAI.VectorStores;
using WebApi.Data.Entities;
using WebApi.Extensions;
using WebApi.Repositories.Note;
using WebApi.Services.Agent.Dtos;
using WebApi.Services.Http;
using WebApi.Services.Parsers;

namespace WebApi.Services.Agent.Agents;

public interface ILexAgent : IChatAgent<PromptContracts.PromptNoteCreationDto, PromptContracts.LexResponseDto>;

public class Lex : ILexAgent
{
    public Lex(AssistantClient assistantClient, ToolRouter toolRouter, IOptions<OpenAiSettings> options, BlockNoteParserService blockNoteParserService, ILogger<Lex> logger, INoteRepository noteRepository, UserContext<User, string> userContext, UnitOfWork unitOfWork, VectorStoreClient vectorStoreClient)
    {
        _assistantClient = assistantClient;
        _toolRouter = toolRouter;
        _blockNoteParserService = blockNoteParserService;
        _logger = logger;
        _noteRepository = noteRepository;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _vectorStoreClient = vectorStoreClient;
        _assistantId = options.Value.LexAssistantId;
    }

    public string AgentName => nameof(Lex);
    
    private readonly AssistantClient _assistantClient;
    private readonly ToolRouter _toolRouter;
    private readonly string _assistantId;
    private readonly BlockNoteParserService _blockNoteParserService;
    private readonly INoteRepository _noteRepository;
    private readonly UserContext<User, string> _userContext;
    private readonly ILogger<Lex> _logger;
    private readonly UnitOfWork _unitOfWork;
    private readonly VectorStoreClient _vectorStoreClient;
    public async Task<PromptContracts.LexResponseDto> ProcessPromptAsync(PromptContracts.PromptNoteCreationDto prompt)
    {
        var message = prompt.PromptMessage;
        
        

        AssistantThread thread;
        if (prompt.FileIds.Any())
        {
            var vectorStoreOption = new VectorStoreCreationOptions();

            foreach (var fileId in prompt.FileIds)
            {
                vectorStoreOption.FileIds.Add(fileId);
            }
            
            var store = await _vectorStoreClient.CreateVectorStoreAsync(waitUntilCompleted: true, vectorStoreOption);
            
            if(store.Value is null) throw new Exception("Could not create vector store.");
            
            var fsResources = new FileSearchToolResources();
            fsResources.VectorStoreIds.Add(store.Value!.Id);
            
            thread = await _assistantClient.CreateThreadAsync(new ThreadCreationOptions()
            {
                InitialMessages =
                {
                    message
                },
                ToolResources = new ToolResources
                {
                    FileSearch = fsResources
                }
            });    
        }
        else
        {
            thread = await _assistantClient.CreateThreadAsync(new ThreadCreationOptions()
            {
                InitialMessages =
                {
                    message
                },
            });
        }
        
        
        var streamingUpdates = _assistantClient.CreateRunStreamingAsync(
            thread.Id,
            _assistantId
        );

        await foreach (StreamingUpdate streamingUpdate in streamingUpdates)
        {
            if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCreated)
            {
                _logger.LogInformation($"--- Run started! ---");
            }
            if (streamingUpdate is MessageContentUpdate contentUpdate)
            {
                _logger.LogInformation(contentUpdate.Text);
            }

            if (streamingUpdate is RequiredActionUpdate requiredActionUpdate)
            {
                var action = requiredActionUpdate.GetThreadRun().RequiredActions.First();
                _logger.LogInformation($"Performing {action.FunctionName}");
            }

            if (streamingUpdate is RunStepDetailsUpdate stepUpdate)
            {
                var step = stepUpdate.FileSearchResults.FirstOrDefault();
                if (step is not null && step.FileName is not null && step.FileName.Contains("file_search") &&
                    step.FileName.Contains("update"))
                {
                    _logger.LogInformation($"file search update {step.FileName}");
                }
            }

            if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCompleted)
            {
                break;
            }
        }
        
        var messages = _assistantClient.GetMessages(thread.Id);
        var messageItem  = messages.First();
        var content = messageItem.Content.First().Text;
        
        var result = JsonConvert.DeserializeObject<PromptContracts.PromptNoteCreationResponseDto>(content)!;
        
        var currentUser = await _userContext.GetCurrentUser();
        var createdNote = Note.Create(currentUser, result.Title, result.Content);
        
        _noteRepository.Add(createdNote);
        
        await _unitOfWork.Commit(CancellationToken.None);

        return new PromptContracts.LexResponseDto
        {
            NoteId = createdNote.Id,
            Title = createdNote.Title
        };
    }
}