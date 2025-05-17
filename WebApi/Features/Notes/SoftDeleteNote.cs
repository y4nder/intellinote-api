using MediatR;
using WebApi.Data.Entities;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.Notes;

public class SoftDeleteNote
{
    public class SoftDeleteCommand : IRequest<Result<SoftDeleteResponse>>
    {
        public Guid NoteId { get; set; }   
    }

    public class SoftDeleteResponse
    {
        public Guid NoteId { get; set; }  
    }
    
    internal sealed class Handler : IRequestHandler<SoftDeleteCommand, Result<SoftDeleteResponse>>
    {
        private readonly NoteRepository _noteRepository;
        private readonly UserContext<User, string> _userContext;
        private readonly UnitOfWork _unitOfWork;

        public Handler(NoteRepository noteRepository, UserContext<User, string> userContext, UnitOfWork unitOfWork)
        {
            _noteRepository = noteRepository;
            _userContext = userContext;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<SoftDeleteResponse>> Handle(SoftDeleteCommand request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.FindByIdAsync(request.NoteId);

            var isOwned = OwnershipValidator.Ensure(note, n => n.UserId == _userContext.Id());
            
            if(isOwned.IsFailure) return Result.Failure<SoftDeleteResponse>(isOwned.Error!);
            
            note!.IsDeleted = true;
            
            await _unitOfWork.Commit(cancellationToken);
            
            return Result.Success(new SoftDeleteResponse
            {
                NoteId = note.Id
            });
        }
    }
}