using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;
using WebApi.Generics;
using WebApi.Services.Parsers;

namespace WebApi.Data.Entities;

public class Note : Entity<Guid>
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string NormalizedContent { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    
    [ForeignKey("FolderId")]
    public Guid? FolderId { get; set; }
    public Folder? Folder { get; set; }
    public List<string> Keywords { get; set; } = new();
    public List<string> Topics { get; set; } = new();
    public string? Mindmap { get; set; } 
    
    //Embedding
    [Column(TypeName = "vector(1536)")]
    public Vector? Embedding { get; set; }

    public string? Summary { get; set; }
    public bool IsDeleted { get; set; } = false;
    
    public static int EmbeddingDimensions = 1536;

    public Note() { }

    private Note(
        Guid id,
        User user, 
        string title, 
        string content,
        string normalizedContent,
        string? mindmap = null
    )
    {
        Title = title;
        Id = id;
        UserId = user.Id;
        User = user;
        Content = content;
        NormalizedContent = normalizedContent;
        Mindmap = mindmap;
    }

    public static Note Create(
        User user, 
        String title, 
        String content)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            title = "Untitled Note";
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            content = "[{\"type\":\"paragraph\"}]";
        }
        
        var note =  new Note(
            Guid.NewGuid(), 
            user, 
            title, 
            content,
            "");
        note.ForceUpdate();
        return note;
    }
    
    public void Update(Note note)
    {
        Title = note.Title;
        Content = note.Content;
        Summary = note.Summary;
        SetUpdated();
    }

    public void SetEmbedding(Vector embedding)
    {
        Embedding = embedding;
        SetUpdated();
    }

    public void ForceUpdate() => SetUpdated();
        
}

public class NoteDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public AuthorDto Author { get; set; } = null!;
    public FolderDto? Folder { get; set; }
    public List<string> Keywords { get; set; } = new();
    public List<string> Topics { get; set; } = new();
    public string? Mindmap { get; set; }
    public bool IsDeleted { get; set; }
}

public class NoteDtoMinimal
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Summary { get; set; }
    public FolderDto? Folder { get; set; }
    public List<string> Keywords { get; set; } = new();
    public List<string> Topics { get; set; } = new();
    public BlockSnippet? Snippet { get; set; } = null;
    public string? Mindmap { get; set; }
    public bool IsDeleted { get; set; }
}

public class NoteDtoVeryMinimal
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Summary { get; set; }
    public bool IsDeleted { get; set; }
}

public class NoteDtoWithTopics
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<string> Topics { get; set; } = new();
    public bool IsDeleted { get; set; }
}

public class NoteNormalizedDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string NormalizedContent { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public bool IsDeleted { get; set; }
}
