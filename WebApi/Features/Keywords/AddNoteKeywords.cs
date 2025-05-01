using FluentValidation;
using MediatR;
using WebApi.Data.Entities;
using WebApi.Errors;
using WebApi.Errors.ErrorDefinitions;
using WebApi.Features.Keywords.Notifications;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;

namespace WebApi.Features.Keywords;


public class AddNoteKeywords
{
    public class AddNoteKeywordRequest
    {
        public List<AddKeywordDto> Keywords { get; set; } = null!;
    }
    
    public class AddNoteKeywordsCommand : IRequest<Result<AddNoteKeywordsResponse>> 
    {
        public Guid NoteId { get; set; }
        public List<AddKeywordDto> Keywords { get; set; } = null!;
    }

    public class AddNoteKeywordsResponse
    {
        public String Message { get; set; } = null!;
    }

    public class Validator : AbstractValidator<AddNoteKeywordsCommand>
    {
        public Validator()
        {
            RuleFor(r => r.Keywords)
                .NotNull()
                .WithMessage("Keywords list must not be null.")
                .NotEmpty()
                .WithMessage("Keywords list must not be empty.");

            RuleFor(r => r.Keywords)
                .Must(k => k.Where(n => n.Name != null).Select(n => n.Name).Distinct().Count() == k.Count(n => n.Name != null))
                .WithMessage("Keyword names must be unique if provided.");

            RuleFor(r => r.Keywords)
                .Must(k => k.Where(n => n.KeywordId.HasValue).Select(n => n.KeywordId).Distinct().Count() == k.Count(n => n.KeywordId.HasValue))
                .WithMessage("Keyword IDs must be unique if provided.");
        }
    }

    
    internal sealed class Handler : IRequestHandler<AddNoteKeywordsCommand, Result<AddNoteKeywordsResponse>>
    {
        private readonly UserContext<User, string> _userContext;
        private readonly KeywordRepository _keywordRepository;
        private readonly NoteRepository _noteRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IValidator<AddNoteKeywordsCommand> _validator;
        
        public Handler(NoteRepository noteRepository,
            KeywordRepository keywordRepository,
            UserContext<User, string> userContext,
            UnitOfWork unitOfWork, IMediator mediator, 
            IValidator<AddNoteKeywordsCommand> validator)
        {
            _noteRepository = noteRepository;
            _keywordRepository = keywordRepository;
            _userContext = userContext;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _validator = validator;
        }

        public async Task<Result<AddNoteKeywordsResponse>> Handle(AddNoteKeywordsCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Failure<AddNoteKeywordsResponse>(validationResult.Errors.ExtractToList());
            
            var note = await _noteRepository.FindByIdAsync(command.NoteId);

            //Ensure note ownership
            var owned = OwnershipValidator.Ensure(
                note, 
                n => n.User.Id == _userContext.Id()
            );
            
            if(owned.IsFailure) 
                return Result.Failure<AddNoteKeywordsResponse>(owned.Error!);
            
            var keywordIds = ExtractKeywordIds(command);
            var keywordNames = ExtractKeywordNames(command);

            var existingKeywords = await _keywordRepository
                .GetExistingKeywords(keywordIds, keywordNames, cancellationToken);
            
            // list of note retrieved keyword names
            var keywordsToCreate = keywordNames
                .Where(k => existingKeywords.All(ek => ek.Name != k))
                .ToList();

            // if notes keywords has ids that match in the existing keywords
            if (HasMatchingKeywords(note, existingKeywords))
            {
                return Result.Failure<AddNoteKeywordsResponse>(KeywordErrors.KeywordAlreadyExists);
            }
            
            if (existingKeywords.Any())
            {
                note!.AddKeywords(existingKeywords);
                var saved = await _unitOfWork.Commit(cancellationToken);
                if(saved.IsFailure) return Result.Failure<AddNoteKeywordsResponse>(saved.Error!);
            } 
            
            if (keywordsToCreate.Any())
            {
                var createKeywordDtos = keywordsToCreate.
                    Select(name => new CreateKeywordDto
                    {
                        Name = name
                    }).ToList();
                
                //publish mediator notification for creating batch notes
                await _mediator.Publish(new CreateBatchKeywordsNotification
                {
                    NoteId = note!.Id,
                    Keywords = createKeywordDtos
                }, cancellationToken);
            }
            
            if(!existingKeywords.Any() && !keywordsToCreate.Any())
            {
                return Result.Failure<AddNoteKeywordsResponse>(KeywordErrors.InvalidKeywordIds);
            }
            
            return Result.Success(new AddNoteKeywordsResponse
            {
                Message = "Added note Keywords"
            });
        }

        private static bool HasMatchingKeywords(Note? note, List<Keyword> existingKeywords)
        {
            // return note!.Keywords.Any(k => existingKeywords.Any(ek => ek.Id == k.Id || ek.Name == k.Name));
            return false;
        }

        private static List<string> ExtractKeywordNames(AddNoteKeywordsCommand command)
        {
            List<string> keywordNames = command.Keywords
                .Where(k => !string.IsNullOrWhiteSpace(k.Name))
                .Select(k => k.Name!.ToUpper()).ToList();
            return keywordNames;
        }

        private static List<Guid> ExtractKeywordIds(AddNoteKeywordsCommand command)
        {
            List<Guid> keywordIds = command.Keywords
                .Where(k => k.KeywordId.HasValue)
                .Select(k => k.KeywordId!.Value).ToList();
            return keywordIds;
        }
    }
}