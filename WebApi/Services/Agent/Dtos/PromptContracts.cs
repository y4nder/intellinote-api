using NetTopologySuite.Geometries.Prepared;

namespace WebApi.Services.Agent.Dtos;

public class PromptContracts
{
    public class PromptRequestDto
    {
        public string? ThreadId { get; set; }
        public string PromptMessage { get; set; } = null!;
        public PromptContext PromptContext { get; set; } = new();
    }

    public class PromptContext
    {
        public Guid? NoteId { get; set; }
        public Guid? FolderId { get; set; }
    }

    public class PromptResponseDto
    {
        public string ThreadId { get; set; } = null!;
        public string ResponseMessage { get; set; } = null!;
        public List<NoteCitation> NoteCitations { get; set; } = new();
        public List<FolderCitation> FolderCitations { get; set; } = new();
    }

    public class NoteCitation
    {
        public Guid NoteId { get; set; }
        public string? SnippetId { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class FolderCitation
    {
        public string? FolderId { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class PromptNoteCreationDto
    {
        public string PromptMessage { get; set; } = null!;
        public List<string> FileIds { get; set; } = [];
    }
    
    public class PromptNoteCreationResponseDto
    {
        public string Message { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
    
    public class LexResponseDto
    {
        public Guid NoteId { get; set; }
        public string Title { get; set; } = null!;

    }
}