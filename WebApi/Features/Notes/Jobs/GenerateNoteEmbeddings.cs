using System.Diagnostics;
using Quartz;
using WebApi.Repositories;
using WebApi.Repositories.Note;
using WebApi.Services;
using WebApi.Services.Hubs;

namespace WebApi.Features.Notes.Jobs;

public class GenerateNoteEmbeddings : IJob
{
    private readonly INoteRepository _noteRepository;
    private readonly UnitOfWork _unitOfWork;
    private readonly EmbeddingService _embeddingService;
    private readonly NoteHubService _noteHubService;
    public const String Name = nameof(GenerateNoteEmbeddings);

    public GenerateNoteEmbeddings(
        EmbeddingService embeddingService,
        INoteRepository noteRepository,
        UnitOfWork unitOfWork, 
        NoteHubService noteHubService)
    {
        _embeddingService = embeddingService;
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
        _noteHubService = noteHubService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var stopWatch = Stopwatch.StartNew();
        var contextMergedJobDataMap = context.MergedJobDataMap;
        
        var noteId = contextMergedJobDataMap.GetString("noteId");
        var textToEmbed = contextMergedJobDataMap.GetString("textToEmbed");

        if (string.IsNullOrEmpty(noteId)) return;
        if (string.IsNullOrEmpty(textToEmbed)) return;
        
        var parsedId = Guid.Parse(noteId);
        
        var textEmbeddingsVector = await _embeddingService.GenerateEmbeddings(textToEmbed);

        var note = await _noteRepository.FindByIdAsync(parsedId);
        if(note == null) return;
        
        note.SetEmbedding(textEmbeddingsVector);
        note.NormalizedContent = textToEmbed;
        
        await _unitOfWork.Commit(CancellationToken.None);
        stopWatch.Stop();
        await _noteHubService.NotifyEmbeddingDone(note, stopWatch.ElapsedMilliseconds);
    }
}