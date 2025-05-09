using MediatR;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using WebApi.Data.Entities;
using WebApi.Errors.ErrorDefinitions;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.ResultType;
using WebApi.Services;
using WebApi.Services.Parsers;

namespace WebApi.Features.Notes.AutoAssignNote;

public class GetPotentialFolders
{
    public class AssignNoteFolderRequest : IRequest<Result<AssignNoteFolderResponse>>
    {
        public Guid NoteId { get; set; }
    }

    public class AssignNoteFolderResponse
    {
        public List<FolderScore> Scores { get; set; } = new();
    }
    
    internal sealed class Handler : IRequestHandler<AssignNoteFolderRequest, Result<AssignNoteFolderResponse>>
    {
        private readonly NoteRepository _noteRepository;
        private readonly FolderRepository _folderRepository;
        private readonly BlockNoteParserService _blockNoteParserService;
        private readonly UserContext<User, string> _userContext;

        public Handler(NoteRepository noteRepository, BlockNoteParserService blockNoteParserService, UserContext<User, string> userContext, FolderRepository folderRepository)
        {
            _noteRepository = noteRepository;
            _blockNoteParserService = blockNoteParserService;
            _userContext = userContext;
            _folderRepository = folderRepository;
        }

        public async Task<Result<AssignNoteFolderResponse>> Handle(AssignNoteFolderRequest request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.FindByIdAsync(request.NoteId);            
            var owned = OwnershipValidator.Ensure(note, n => n.UserId == _userContext.Id());
            
            if(owned.IsFailure) return Result.Failure<AssignNoteFolderResponse>(owned.Error!);

            // if no embedding or length too short return error
            var noteContent = _blockNoteParserService.PrepareNoteForEmbedding(note!);
            if (noteContent is null || noteContent.Length < 100 || note!.Embedding is null)
            {
                return Result.Failure<AssignNoteFolderResponse>(NoteErrors.NotEnoughContent);
            }
            
            // start searching
            var dbSet = _folderRepository.GetDbSet();
            var noteVector = note!.Embedding;
            var topSemanticFolders = await dbSet
                .Where(f => f.Embedding != null)
                .OrderBy(f => f.Embedding!.CosineDistance(noteVector))
                .ThenByDescending(f => f.UpdatedAt)
                .Take(50)
                .ToListAsync(cancellationToken: cancellationToken);
            
            // computing for keyword match and final score in memory
            var noteKeywords = new HashSet<string>(note.Keywords);

            var scoredFolders = topSemanticFolders
                .Select(folder =>
                {
                    var folderKeywords = new HashSet<string>(folder.Keywords);
                    var keywordScore = KeywordSimilarity.Compute(noteKeywords.ToList(), folderKeywords.ToList());
                    var cosineSimilarity = InMemoryCosineSimilarity.Compute(
                        folder.Embedding!.ToArray(), 
                        noteVector.ToArray()
                    );
                    var finalScore = 0.9 * cosineSimilarity + 0.1 * keywordScore;

                    return new FolderScore
                    {
                        FolderId = folder.Id,
                        FolderName = folder.Name,
                        Score = finalScore
                    };
                })
                .OrderByDescending(f => f.Score)
                .Take(10)
                .ToList();

            
            return Result.Success(new AssignNoteFolderResponse
            {
                Scores = scoredFolders
            });

        }

    }
    public class FolderScore{
        public Guid FolderId { get; set; }
        public string FolderName { get; set; } = null!;
        public double Score { get; set; }
    }
            
}