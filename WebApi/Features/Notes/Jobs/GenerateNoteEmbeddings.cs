using Quartz;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApi.Features.Notes.Jobs;

public class GenerateNoteEmbeddings : IJob
{
    private readonly NoteRepository _noteRepository;
    private readonly UnitOfWork _unitOfWork;
    private readonly EmbeddingService _embeddingService;
    public const String Name = nameof(GenerateNoteEmbeddings);

    public GenerateNoteEmbeddings(
        EmbeddingService embeddingService, NoteRepository noteRepository, UnitOfWork unitOfWork)
    {
        
        _embeddingService = embeddingService;
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var contextMergedJobDataMap = context.MergedJobDataMap;
        
        var noteId = contextMergedJobDataMap.GetString("noteId");
        var textToEmbed = contextMergedJobDataMap.GetString("textToEmbed");

        if (string.IsNullOrEmpty(noteId)) return;
        if (string.IsNullOrEmpty(textToEmbed)) return;
        
        var parsedId = Guid.Parse(noteId);
        
        var textEmbeddingsVector = await _embeddingService.GenerateEmbeddings(textToEmbed);

        var note = await _noteRepository.FindByIdAsync(parsedId);
        if(note == null) return;
        
        note.Embedding = textEmbeddingsVector;
        
        await _unitOfWork.Commit(CancellationToken.None);
    }
}