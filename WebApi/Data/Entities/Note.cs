using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;
using WebApi.Generics;

namespace WebApi.Data.Entities;

public class Note : Entity<Guid>
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string UserId { get; set; }
    public User User { get; set; } 
    public Folder? Folder { get; set; }
    public List<Keyword> Keywords { get; set; } = new List<Keyword>();
    
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
        return new Note(
            Guid.NewGuid(), 
            user, 
            title, 
            content);
    }

    public void AddKeywords(List<Keyword> keywords)
    {
        
        
        if (Keywords.Count == 0)
        {
            Keywords = keywords;
        }
        else
        {
            Keywords.AddRange(keywords);
        }
        SetUpdated();
    }

    public void Update(Note note)
    {
        Title = note.Title;
        Content = note.Content;
        SetUpdated();
    }

    public List<Keyword> GetKeywords() => Keywords.AsReadOnly().ToList();
}

public class NoteDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public AuthorDto Author { get; set; } = null!;
    public FolderDto? Folder { get; set; }
    public List<KeywordDto> Keywords { get; set; } = new();
}

public class NoteDtoMinimal
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<KeywordDto> Keywords { get; set; } = new();
}