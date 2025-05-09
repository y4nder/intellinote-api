using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;
using WebApi.Generics;

namespace WebApi.Data.Entities;

public class Folder : Entity<Guid>
{
    public string Name { get; set; }
    public string UserId { get; set; }
    public string Description { get; set; }
    public User User { get; set; }
    
    public List<Note> Notes { get; set; }
    public List<string> Keywords { get; set; }
    public List<string> Topics { get; set; }
    
    //Embedding
    [Column(TypeName = "vector(1536)")]
    public Vector? Embedding { get; set; }
    
    public static int EmbeddingDimensions = 1536;

    public Folder() { }
    
    private Folder(Guid id, string name, string description, User user, List<Note>? notes = null, List<string>? keywords = null, List<string>? topics = null)
    {
        Id = id;
        Name = name;
        Description = description;
        UserId = user.Id;
        User = user;
        Notes = (notes ?? notes) ?? new List<Note>();
        Keywords = (keywords ?? keywords) ?? new List<string>();
        Topics = (topics ?? topics) ?? new List<string>();
    }

    public static Folder Create(string name, string description, User user)
    {
        if (string.IsNullOrEmpty(description))
        {
            description = "";
        }
        return new Folder(Guid.NewGuid(), name, description, user);
    }
    
    public void AddNotes(List<Note> notes)
    {
        if (Notes.Count == 0) Notes = notes;
        else Notes.AddRange(notes);
        
        var keywords = notes.SelectMany(n => n.Keywords).Distinct().ToList();
        if (Keywords.Count == 0) Keywords = keywords;
        else Keywords.AddRange(keywords);
        
        var topics = notes.SelectMany(n => n.Topics).Distinct().ToList();
        if (Topics.Count == 0) Topics = topics;
        else Topics.AddRange(topics);
        
        SetUpdated();
    }

    public void RemoveNotes(List<Note> notes)
    {
        var noteIdsToRemove = notes.Select(n => n.Id).ToList();
        Notes.RemoveAll(n => noteIdsToRemove.Contains(n.Id));

        // Remove keywords that are only associated with the removed notes
        var keywordsToRemove = notes.SelectMany(n => n.Keywords).Distinct().ToList();
        Keywords.RemoveAll(k => keywordsToRemove.Contains(k));
        SetUpdated();
    }
}

public class FolderDto
{
    public Guid Id { get; set; }
    public String Name { get; set; }
};

public class FolderWithDetailsDto
{
    public Guid Id { get; set; }
    public String Name { get; set; }
    public string Description { get; set; }
    public AuthorDto Author { get; set; }
    public List<NoteDtoMinimal> Notes { get; set; }
    public List<string> Keywords { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; }
}

