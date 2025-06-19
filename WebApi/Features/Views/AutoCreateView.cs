using MediatR;
using WebApi.Data.Entities;
using WebApi.Repositories;
using WebApi.Repositories.Note;
using WebApi.Repositories.View;
using WebApi.Services;
using WebApi.Services.Http;

namespace WebApi.Features.Views;

public class AutoCreateView 
{
    public class Request : IRequest<ViewResponseDto>
    {
        public string Query { get; set; } = null!;
    }

    internal sealed class Handler : IRequestHandler<Request, ViewResponseDto>
    {
        private readonly INoteRepository _noteRepository;
        private readonly UserContext<User, string> _userContext;
        private readonly EmbeddingService _embeddingService;
        private readonly TopicExtractorService _topicExtractorService;
        private readonly IViewRepository _viewRepository;
        private readonly UnitOfWork _unitOfWork;

        public Handler(INoteRepository noteRepository,
            UserContext<User, string> userContext,
            EmbeddingService embeddingService,
            TopicExtractorService topicExtractorService, IViewRepository viewRepository, UnitOfWork unitOfWork)
        {
            _noteRepository = noteRepository;
            _userContext = userContext;
            _embeddingService = embeddingService;
            _topicExtractorService = topicExtractorService;
            _viewRepository = viewRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ViewResponseDto> Handle(Request request, CancellationToken cancellationToken)
        {
            var searchVector = await _embeddingService.GenerateEmbeddings(request.Query);

            var relevantNotes = await _noteRepository.SearchNotesForAgentWithTopics(_userContext.Id(), searchVector);
            
            var extractionResponse = await _topicExtractorService.ExtractTopics(request.Query, relevantNotes);

            var createdView = View.CreateManually(
                _userContext.GetCurrentUser().Result,
                extractionResponse.Name,
                extractionResponse.Topics
            );
            
            _viewRepository.Add(createdView);
            await _unitOfWork.Commit(cancellationToken);

            return new ViewResponseDto
            {
                Id = createdView.Id,
                Name = createdView.Name,
                FilterCondition = createdView.FilterCondition,
            };
        }
    }
}