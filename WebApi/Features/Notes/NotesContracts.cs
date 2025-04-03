using MediatR;
using WebApi.Data.Entities;
using WebApi.Generics;
using WebApi.ResultType;

namespace WebApi.Features.Notes;

public class NotesContracts
{
    public record GetNotesRequest() : IRequest<Result<GetNotesResponse>>;
    public record GetNoteResponse(NoteDto Note);
    public record GetNotesResponse(
        List<NoteDto> Notes
    );
    
    public record CreateNoteRequest(
        String Title,
        String Content
    ) : IMappable<CreateNote.Command>;

    public record CreateNoteResponse(NoteDto Note);

    public record UpdateNoteRequest(
        String Title,
        String Content
) : IMappable<UpdateNote.Command>;
    
    
    public record UpdateNoteResponse(NoteDto Note);
    
    public record DeleteNoteResponse(Guid NoteId);
    
    
    
}