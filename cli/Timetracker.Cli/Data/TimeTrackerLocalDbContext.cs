using Microsoft.EntityFrameworkCore;
using Timetracker.Core.Models;

namespace Timetracker.Cli.Data;

public sealed class TimeTrackerLocalDbContext : DbContext
{
    public TimeTrackerLocalDbContext(DbContextOptions<TimeTrackerLocalDbContext> options)
        : base(options)
    {
    }

    public DbSet<TrackedTask> TrackedTasks => Set<TrackedTask>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackedTask>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired();

            entity.HasQueryFilter(t => t.DeletedAtUtc == null);

            entity.HasMany(t => t.TimeSlots)
                  .WithOne(s => s.Task!)
                  .HasForeignKey(s => s.TrackedTaskId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasQueryFilter(s => s.DeletedAtUtc == null);
        });
    }
}
