using Microsoft.EntityFrameworkCore;
using WebApi.Data.Entities;

namespace WebApi.Extensions;

public static class OnModelCreateExtensions
{
    public static ModelBuilder UseModelCreateExtension(this ModelBuilder modelBuilder)
    {
        // add keyword extensions here
        modelBuilder.KeywordExtensions();
        modelBuilder.NoteExtensions();
        
        return modelBuilder;
    }
    
    public static ModelBuilder SampleEntityExtension(this ModelBuilder modelBuilder){
        modelBuilder.Entity<SampleEntity>(c => {
            c.HasIndex(e => e.UniqueName).IsUnique();
        });
        return modelBuilder;
    }

    private static ModelBuilder KeywordExtensions(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Keyword>(k =>
        {
            k.HasIndex(e => e.Name).IsUnique();
        });
        return modelBuilder;
    }

    private static ModelBuilder NoteExtensions(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>()
            .HasMany(n => n.Keywords)
            .WithMany(k => k.Notes)
            .UsingEntity<KeywordNote>();
        
        modelBuilder.Entity<KeywordNote>()
            .HasOne(kn => kn.Keyword)
            .WithMany()
            .HasForeignKey(kn => kn.KeywordId);
        
        modelBuilder.Entity<KeywordNote>()
            .HasOne(kn => kn.Note)
            .WithMany()
            .HasForeignKey(kn => kn.NoteId);
        
        return modelBuilder;
    }

    private static ModelBuilder KeywordNoteExtensions(this ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<KeywordNote>()
        //     .HasKey(kn => new { kn.NoteId, kn.KeywordId });
        //
        // modelBuilder.Entity<KeywordNote>()
        //     .HasOne(kn => kn.Note)
        //     .WithMany(n => n.KeywordNotes)
        //     .HasForeignKey(kn => kn.NoteId);
        //
        // modelBuilder.Entity<KeywordNote>()
        //     .HasOne(kn => kn.Keyword)
        //     .WithMany(k => k.KeywordNotes)
        //     .HasForeignKey(kn => kn.KeywordId);
        return modelBuilder;
    }
}
