using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL;
using Aufy.Core;
using Aufy.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Data.Entities;
using WebApi.Extensions;
using WebApi.Generics;

namespace WebApi.Data;

public class ApplicationDbContext : IdentityDbContext<User>, IAufyDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // public DbSet<SampleEntity> SampleEntities { get; set; }
    public DbSet<AufyRefreshToken> RefreshTokens { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<Keyword> Keywords { get; set; }
    public DbSet<KeywordNote> KeywordNotes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        // optionsBuilder.UseNpgsql()
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasPostgresExtension("vector");
        builder.ApplyAufyModel();
        builder.Ignore<DomainEvent>();
        builder.UseModelCreateExtension();
        builder.AddQuartz(q => q.UsePostgreSql());
        // builder.SampleEntityExtension();
    }
}