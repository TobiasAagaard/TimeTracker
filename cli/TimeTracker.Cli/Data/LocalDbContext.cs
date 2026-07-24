using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using TimeTracker.Core.Models;

namespace TimeTracker.Cli.Data;

public sealed class LocalDbContext : DbContext
{
    public DbSet<TrackedTasks> TrackedTasks => Set<TrackedTasks>();
    public DbSet<TimeSlots> TimeSlots => Set<TimeSlots>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var dbPath = ResolveDatabasePath();
        var directory = Path.GetDirectoryName(dbPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath
        }.ToString();

        optionsBuilder.UseSqlite(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackedTasks>(entity => 
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired();
            entity.HasQueryFilter(t => t.DeletedAt == null);

            entity.HasMany(t => t.TimeSlots)
                    .WithOne(s => s.Task)
                    .HasForeignKey(s => s.TrackedTaskId)
                    .OnDelete(DeleteBehavior.Cascade);


        });

        modelBuilder.Entity<TimeSlots>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasQueryFilter(s => s.DeletedAt == null);
        });
    }

    private static string ResolveDatabasePath()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        if (string.IsNullOrWhiteSpace(appDataPath))
        {
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        if (string.IsNullOrWhiteSpace(appDataPath))
        {
            appDataPath = AppContext.BaseDirectory;
        }

        return Path.GetFullPath(Path.Combine(appDataPath, "TimeTracker", "TimeTracker.db"));
    }
}