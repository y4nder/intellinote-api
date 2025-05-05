using System.Diagnostics;
using MediatR;
using Quartz;
using WebApi.Data.Entities;
using WebApi.Features.Notes.Notification;
using WebApi.Repositories;
using WebApi.Services;
using WebApi.Services.External;
using WebApi.Services.Hubs;

namespace WebApi.Features.Notes.Jobs;

public class GenerateKeywordAndSummaryJob : IJob
{
    private readonly NoteHubService _noteHubService;
    private readonly NoteRepository _noteRepository;
    private readonly UnitOfWork _unitOfWork;
    private readonly GeneratedResponseService _generatedResponseService;
    private readonly IMediator _mediator;
    public const String Name = nameof(GenerateKeywordAndSummaryJob);

    public GenerateKeywordAndSummaryJob(NoteHubService noteHubService, NoteRepository noteRepository, GeneratedResponseService generatedResponseService, UnitOfWork unitOfWork, IMediator mediator)
    {
        _noteHubService = noteHubService;
        _noteRepository = noteRepository;
        _generatedResponseService = generatedResponseService;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var stopWatch = Stopwatch.StartNew();
        var contextMergedJobDataMap = context.MergedJobDataMap;
        
        var noteId = contextMergedJobDataMap.GetString("noteId");
        var text = contextMergedJobDataMap.GetString("text");

        if (string.IsNullOrEmpty(noteId)) return;
        if (string.IsNullOrEmpty(text)) return;
        
        var parsedId = Guid.Parse(noteId);
        var note = await _noteRepository.FindByIdAsync(parsedId);
        
        if(note == null) return;

        GeneratedResponse? generatedResponse;
        try
        {
            generatedResponse = await _generatedResponseService.GetGeneratedResponse(text);
        }
        catch (Exception e)
        {
            stopWatch.Stop();
            await _noteHubService.NotifyGenerationFailed(
                $"Failed to generate response for note {note.Id}", 
                stopWatch.ElapsedMilliseconds);
            return;
        }
        
        note.Summary = generatedResponse.Summary;
        note.Topics = generatedResponse.Topics;
        note.Keywords = generatedResponse.Keywords.Select(k => k.Keyword).ToList();

        await _unitOfWork.Commit(CancellationToken.None);
        stopWatch.Stop();
        await _noteHubService.NotifyGenerationDone(note, generatedResponse, stopWatch.ElapsedMilliseconds);
        await HandleEmbeddings(note, CancellationToken.None);
    }
    
    private async Task HandleEmbeddings(Note note, CancellationToken cancellationToken)
    {
        var textToEmbed = $"{note.Title} {note.Content} {note.Summary}";
        await _mediator.Publish(new GenerateNoteEmbeddingsNotification
        {
            NoteId = note.Id,
            TextToEmbed = textToEmbed
        }, cancellationToken);
    }
}