using MediatR;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using WebApi.Data.Entities;
using WebApi.Errors.ErrorDefinitions;
using WebApi.Features.Utilities;
using WebApi.Repositories;
using WebApi.Repositories.Folder;
using WebApi.Repositories.Note;
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
        public PotentialFolder Folder { get; set; } = null!;
    }
    
    internal sealed class Handler : IRequestHandler<AssignNoteFolderRequest, Result<AssignNoteFolderResponse>>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly BlockNoteParserService _blockNoteParserService;
        private readonly UserContext<User, string> _userContext;
        private readonly FolderLlmChoiceService _folderLlmChoiceService;

        public Handler(INoteRepository noteRepository,
            BlockNoteParserService blockNoteParserService,
            UserContext<User, string> userContext,
            IFolderRepository folderRepository, FolderLlmChoiceService folderLlmChoiceService)
        {
            _noteRepository = noteRepository;
            _blockNoteParserService = blockNoteParserService;
            _userContext = userContext;
            _folderRepository = folderRepository;
            _folderLlmChoiceService = folderLlmChoiceService;
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
                .Select(folder => new FolderWithSemantics
                {
                    Folder = folder,
                    CosineDistance = folder.Embedding!.CosineDistance(noteVector)
                })
                .ToListAsync(cancellationToken: cancellationToken);
            
            // computing for keyword match and final score in memory
            var noteKeywords = new HashSet<string>(note.Keywords);

            var scoredFolders = topSemanticFolders
                .Select(folderWithSemantics =>
                {
                    var folderKeywords = new HashSet<string>(folderWithSemantics.Folder.Keywords);
                    var keywordScore = KeywordSimilarity.Compute(noteKeywords.ToList(), folderKeywords.ToList());
                    var cosineDistance = folderWithSemantics.CosineDistance;
                    var cosineSimilarity = 1.0 - cosineDistance;
                    var finalScore = 0.9 * cosineSimilarity + 0.1 * keywordScore;

                    return new FolderScore
                    {
                        FolderId = folderWithSemantics.Folder.Id,
                        FolderName = folderWithSemantics.Folder.Name,
                        FolderDescription = folderWithSemantics.Folder.Description,
                        Score = finalScore
                    };
                })
                .OrderByDescending(f => f.Score)
                .Take(10)
                .ToList();

            var finalFolder = new PotentialFolder
            {
                FolderId = scoredFolders[0].FolderId,
                FolderName = scoredFolders[0].FolderName,
                SuggestedFolderName = null,
                Reason = null!,
            };
            
            if (scoredFolders[0].Score < 0.75)
            {
                var llmResponse = await _folderLlmChoiceService.GetFolderChoice(scoredFolders, note);
                finalFolder.FolderId = llmResponse.FolderId;
                finalFolder.FolderName = llmResponse.FolderName;
                finalFolder.SuggestedFolderName = llmResponse.SuggestedFolderName;
                finalFolder.Reason = llmResponse.Reason;
            }

            
            return Result.Success(new AssignNoteFolderResponse
            {
                 Folder = finalFolder
            });

        }

    }
    public class FolderScore{
        public Guid FolderId { get; set; }
        public string FolderName { get; set; } = null!;
        public string FolderDescription { get; set; } = null!;
        public double Score { get; set; }
    }

    public class FolderWithSemantics
    {
        public Folder Folder { get; set; } = null!;
        public double CosineDistance { get; set; }
    }
            
}