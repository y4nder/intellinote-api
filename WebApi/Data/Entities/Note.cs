using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;
using WebApi.Generics;
using WebApi.Services;
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
    
    //Embedding
    [Column(TypeName = "vector(1536)")]
    public Vector? Embedding { get; set; }

    public string? Summary { get; set; }
    
    public static int EmbeddingDimensions = 1536;

    public Note() { }

    private Note(
        Guid id,
        User user, 
        string title, 
        string content
    )
    {
        Title = title;
        Id = id;
        UserId = user.Id;
        User = user;
        Content = content;
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
        
        return new Note(
            Guid.NewGuid(), 
            user, 
            title, 
            content);
    }

    public void AddKeywords(List<Keyword> keywords)
    {
        throw new NotImplementedException();
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

    public void ForceUpdate()
    {
        SetUpdated();
    }

    public string FlattenNoteForEmbedding()
    {
        var text = $"{Title} {Content} {Summary}";
        if (Topics.Any())
        {
            text += string.Join(' ', Topics);
        }

        if (Keywords.Any())
        {
            text += string.Join(' ', Keywords);
        }
        return TextCleaner.Clean(text);
    }
    
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
}