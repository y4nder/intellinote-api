using MediatR;
using WebApi.Repositories;
using WebApi.Repositories.Note;
using WebApi.Services;

namespace WebApi.Features.MindMaps;

public class CreateMindMap
{
    public class CreateMindMapRequest : IRequest<CreateMindMapResponse>
    {
        public Guid NoteId { get; set; }
    }

    public class CreateMindMapResponse
    {
        public string MindMap { get; set; } = null!;
    }
    
    internal sealed class Handler : IRequestHandler<CreateMindMapRequest, CreateMindMapResponse>
    {
        private readonly INoteRepository _noteRepository;
        
        private readonly MindMapService _mindMapService; 
        private readonly UnitOfWork _unitOfWork;
        

        public Handler(INoteRepository noteRepository,
            MindMapService mindMapService,
            UnitOfWork unitOfWork)
        {
            _noteRepository = noteRepository;
            
            _mindMapService = mindMapService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateMindMapResponse> Handle(CreateMindMapRequest request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.FindByIdAsync(request.NoteId);
            if(note is null) throw new Exception("Note not found");
            var content = note.NormalizedContent;
            var generatedMindMap = await _mindMapService.GenerateMindMap(content);
            note.Mindmap = generatedMindMap;
            await _unitOfWork.Commit(cancellationToken);            
            return new CreateMindMapResponse
            {
                MindMap = generatedMindMap
            };
        }
    }
}