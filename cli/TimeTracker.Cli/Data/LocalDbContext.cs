using Microsoft.EntityFrameworkCore;
using TimeTracker.Core.Models;

namespace TimeTracker.Cli.Data;

public sealed class LocalDbContext : DbContext
{

    public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options)
    {
    }

    public DbSet<TrackedTasks> TrackedTasks => Set<TrackedTasks>();
    public DbSet<TimeSlots> TimeSlots => Set<TimeSlots>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackedTasks>();

        modelBuilder.Entity<TimeSlots>();
    }
}