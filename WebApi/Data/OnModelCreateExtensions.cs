using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WebApi.Data.Entities;

namespace WebApi.Data;

public static class OnModelCreateExtensions
{
    public static ModelBuilder UseModelCreateExtension(this ModelBuilder modelBuilder)
    {
        modelBuilder.NoteExtensions();
        modelBuilder.FolderExtensions();
        
        return modelBuilder;
    }
    
    private static ModelBuilder NoteExtensions(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>()
            .HasOne(n => n.Folder) 
            .WithMany(f => f.Notes) 
            .HasForeignKey(n => n.FolderId) 
            .OnDelete(DeleteBehavior.SetNull);  
        
        modelBuilder.Entity<Note>(n =>
        {
            n.Property(p => p.Embedding)
                .HasColumnType("vector(1536)");
        });

        modelBuilder.Entity<Note>().HasIndex(n => n.Embedding).HasMethod("ivfflat").HasOperators("vector_cosine_ops");
        
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
        
        modelBuilder.Entity<Note>(n =>
        {
            n.Property(t => t.Keywords)
                .HasConversion(
                    v => string.Join(';', v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                )
                .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
        });

        modelBuilder.Entity<Note>(note =>
        {
            note.HasIndex(n => new { n.Title, n.Summary, n.NormalizedContent })
                .HasMethod("GIN")
                .IsTsVectorExpressionIndex("english");
        });

        
        return modelBuilder;
    }

    private static ModelBuilder FolderExtensions(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Folder>(f =>
        {
            f.Property(k => k.Keywords)
                .HasConversion(
                    v => string.Join(';', v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                )
                .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
        });
        
        modelBuilder.Entity<Folder>(n =>
        {
            n.Property(t => t.Topics)
                .HasConversion(
                    v => string.Join(';', v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                )
                .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
            ;
        });
        
        modelBuilder.Entity<Folder>().HasIndex(f => f.Embedding).HasMethod("ivfflat").HasOperators("vector_cosine_ops");
        
        modelBuilder.Entity<Folder>()
            .HasMany(f => f.Notes)  
            .WithOne(n => n.Folder) 
            .HasForeignKey(n => n.FolderId) 
            .OnDelete(DeleteBehavior.SetNull);  
        
        return modelBuilder;
    }
}
