using WebApi.Generics;

namespace WebApi.Data.Entities;

public class Keyword : Entity<Guid>
{
    public string Name { get; set; } = null!;

    public List<Note> Notes { get; set; } = new List<Note>();
    public List<Folder> Folders { get; set; } = new List<Folder>();

    public static Keyword Create(String name)
    {
        return new Keyword
        {
            Id = Guid.NewGuid(), 
            Name = name.ToUpper()
        };
    }
    public Keyword() { }
}

public class KeywordDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
};

public class CreateKeywordDto
{
    public string Name { get; set; } = null!;

    public Keyword ToConcreteKeyword()
    {
        var newKeyword = Keyword.Create(Name);
        return newKeyword;
    }
}

public class AddKeywordDto
{
    public Guid? KeywordId { get; set; }
    public string? Name { get; set; }
}