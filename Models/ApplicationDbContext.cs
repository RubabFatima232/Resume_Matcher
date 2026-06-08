using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ResumeMatcher.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<MatchResult> MatchResults => Set<MatchResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MatchResult>()
            .HasOne(m => m.Resume)
            .WithMany(r => r.MatchResults)
            .HasForeignKey(m => m.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MatchResult>()
            .HasOne(m => m.Job)
            .WithMany(j => j.MatchResults)
            .HasForeignKey(m => m.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}