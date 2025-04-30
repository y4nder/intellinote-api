using MediatR;
using WebApi.Data.Entities;
using WebApi.Generics;
using WebApi.ResultType;

namespace WebApi.Features.Notes;

public class NotesContracts
{
    public record GetNoteResponse(NoteDto Note);
    public record GetNotesRequest(string? Term = null, int Skip = 0, int Take = 10) : IRequest<Result<GetNotesResponse>>;
    public record GetNotesResponse(
        List<NoteDto> Notes,
        int TotalCount
    );
    
    public record CreateNoteRequest(
        String Title,
        String Content
    ) : IMappable<CreateNote.Command>;

    public record CreateNoteResponse(NoteDto Note);

    public record UpdateNoteRequest(
        String Title,
        String Content,
        String Summary
) : IMappable<UpdateNote.Command>;
    
    
    public record UpdateNoteResponse(NoteDto Note);
    
    public record DeleteNoteResponse(Guid NoteId);
    
    
    
}