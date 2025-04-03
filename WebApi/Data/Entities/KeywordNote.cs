namespace WebApi.Data.Entities;

public class KeywordNote
{
    public Guid NoteId { get; set; }
    public Guid KeywordId { get; set; }

    public Note Note { get; set; } = null!;
    public Keyword Keyword { get; set; } = null!;

    public KeywordNote() {}
}