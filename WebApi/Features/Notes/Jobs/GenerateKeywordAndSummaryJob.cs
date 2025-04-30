using System.Diagnostics;
using Quartz;
using WebApi.Repositories;
using WebApi.Services.External;
using WebApi.Services.Hubs;

namespace WebApi.Features.Notes.Jobs;

public class GenerateKeywordAndSummaryJob : IJob
{
    private readonly NoteHubService _noteHubService;
    private readonly NoteRepository _noteRepository;
    private readonly GeneratedResponseService _generatedResponseService;
    public const String Name = nameof(GenerateKeywordAndSummaryJob);

    public GenerateKeywordAndSummaryJob(NoteHubService noteHubService, NoteRepository noteRepository, GeneratedResponseService generatedResponseService)
    {
        _noteHubService = noteHubService;
        _noteRepository = noteRepository;
        _generatedResponseService = generatedResponseService;
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

        var generatedResponse = await _generatedResponseService.GetGeneratedResponse(text!);
        
        note.Summary = generatedResponse.Summary;
        note.Topics = generatedResponse.Topics;


    }
}