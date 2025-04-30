using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pgvector;
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
        modelBuilder.Entity<Note>(n =>
        {
            n.Property(p => p.Embedding)
                .HasColumnType("vector(1536)");
        });

        modelBuilder.Entity<Note>().HasIndex(n => n.Embedding).HasMethod("ivfflat").HasOperators("vector_cosine_ops");
        
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

        modelBuilder.Entity<Note>(n =>
        {
            n.Property(t => t.Topics)
                .HasConversion(
                    v => string.Join(';', v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                )
                .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
            ;
        });
        
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
