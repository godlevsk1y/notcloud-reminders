using Microsoft.EntityFrameworkCore;

namespace NotCloud.Reminders.Persistence;

public sealed class RemindersContext : DbContext
{
    public DbSet<Task> Tasks { get; set; }
    
    public RemindersContext(DbContextOptions<RemindersContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>()
            .HasIndex(t => t.Title)
            .IsUnique();
    }
}