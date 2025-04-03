using EFCore.BulkExtensions;
using Newtonsoft.Json;
using Quartz;
using WebApi.Data;
using WebApi.Data.Entities;

namespace WebApi.Features.Keywords.Jobs;

public class BatchInsertNewKeywords : IJob
{
    private readonly ApplicationDbContext _dbContext;
    public const String Name = nameof(BatchInsertNewKeywords); 
    
    public BatchInsertNewKeywords(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        JobDataMap contextMergedJobDataMap = context.MergedJobDataMap;
        
        var keywords = contextMergedJobDataMap.GetString("keywords");
        var noteId = contextMergedJobDataMap.GetString("noteId");
        if (string.IsNullOrEmpty(keywords)) return;
        if (string.IsNullOrEmpty(noteId)) return;
        
        var deserializedKeywords = JsonConvert.DeserializeObject<List<CreateKeywordDto>>(keywords);
        var parsedNoteId = Guid.Parse(noteId);
        if (deserializedKeywords == null || deserializedKeywords.Count == 0) return;

        List<Keyword> createdKeys = deserializedKeywords.Select(d => d.ToConcreteKeyword()).ToList();
        List<KeywordNote> keywordNotes = createdKeys.Select(kn => new KeywordNote
        {
            KeywordId = kn.Id,
            NoteId = parsedNoteId,
        }).ToList();
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await _dbContext.BulkInsertAsync(createdKeys);
            await _dbContext.BulkInsertAsync(keywordNotes);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }

    }
}