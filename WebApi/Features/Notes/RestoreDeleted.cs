using System.Xml.XPath;
using MediatR;
using WebApi.Data.Entities;
using WebApi.Errors;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.Repositories.Note;
using WebApi.ResultType;
using WebApi.Services;
using WebApi.Services.Http;

namespace WebApi.Features.Notes;

public class RestoreDeleted
{
    public class RestoreDeletedRequest : IRequest<Result<RestoreDeletedResponse>>
    {
        public Guid NoteId { get; set; }
    }

    public class RestoreDeletedResponse
    {
        public Guid NoteId { get; set; }
        public string Title { get; set; } = null!;
    }

    internal sealed class Handler : IRequestHandler<RestoreDeletedRequest, Result<RestoreDeletedResponse>>
    {
        private readonly INoteRepository _noteRepository;
        private readonly UserContext<User, string> _userContext;
        private readonly UnitOfWork _unitOfWork;

        public Handler(INoteRepository noteRepository, UserContext<User, string> userContext, UnitOfWork unitOfWork)
        {
            _noteRepository = noteRepository;
            _userContext = userContext;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RestoreDeletedResponse>> Handle(RestoreDeletedRequest request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.FindDeletedNote(request.NoteId);
            
            var isOwned = OwnershipValidator.Ensure(note, n => n.UserId == _userContext.Id());
            
            if(isOwned.IsFailure) return Result.Failure<RestoreDeletedResponse>(isOwned.Error!);

            if (!note!.IsDeleted)
            {
                return Result.Failure<RestoreDeletedResponse>(new Error("NOTE IS NOT DELETED", "Note is note deleted", StatusCodes.Status400BadRequest));
            }
            
            note.IsDeleted = false;
            
            await _unitOfWork.CommitAsync(cancellationToken);
            
            return Result.Success(new RestoreDeletedResponse
            {
                NoteId = note.Id,
                Title = note.Title
            });
        }
    }
}