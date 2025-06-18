using Pgvector;
using WebApi.Data.Entities;
using WebApi.Generics;

namespace WebApi.Repositories.Note;

public interface INoteRepository : IRepository<Data.Entities.Note, Guid>
{
    Task<List<Data.Entities.Note>> GetNotesByNoteIdsAsync(List<Guid> noteIds);
    Task<List<NoteDtoVeryMinimal>> SearchNotesForAgent(string userId, Vector searchVector, int top = 5);

    Task<List<NoteDtoWithTopics>> SearchNotesForAgentWithTopics(string userId, Vector searchVector,
        int top = 15);

    Task<PaginatedResult<NoteDtoMinimal>> SearchNotesAsync(string userId, string? searchTerm = null,
        int skip = 0, int take = 10);

    Task<PaginatedResult<NoteDtoMinimal>> GetAllNotesForUserAsync(string userId, Vector? searchVector = null,
        int skip = 0, int take = 10);

    Task<NoteNormalizedDto?> GetNormalizedNoteContent(Guid noteId);
    Task<NoteDto?> FindNoteWithProjection(Guid noteId);
    Task<List<NoteDtoMinimal>> FindDeletedNotesWithProjection(string userId);
    Task<Data.Entities.Note?> FindDeletedNote(Guid noteId);
}