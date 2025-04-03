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
        
        return modelBuilder;
    }   
}
