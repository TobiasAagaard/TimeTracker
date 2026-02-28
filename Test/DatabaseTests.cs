using System;
using System.IO;
using Microsoft.Data.Sqlite;
using TimeTracker.Data;
using Xunit;

namespace TimeTracker.Tests;

public class DatabaseTests : IDisposable
{
    private readonly List<string> _tempFiles = new();
    private readonly List<string> _tempDirectories = new();

    public void Dispose()
    {
        foreach (var file in _tempFiles)
        {
            if (File.Exists(file))
            {
                try { File.Delete(file); } catch { }
            }
        }

        foreach (var dir in _tempDirectories)
        {
            if (Directory.Exists(dir))
            {
                try { Directory.Delete(dir, true); } catch { }
            }
        }
    }

    private string CreateTempDatabasePath()
    {
        var path = Path.Combine(Path.GetTempPath(), $"test_db_{Guid.NewGuid()}.db");
        _tempFiles.Add(path);
        return path;
    }

    [Fact]
    public void Constructor_WithNullPath_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Database(null!));
    }

    [Fact]
    public void Constructor_WithEmptyPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Database(""));
    }

    [Fact]
    public void Constructor_CreatesDatabaseAndTimeEntriesTable()
    {
        var dbPath = CreateTempDatabasePath();

        _ = new Database(dbPath);

        Assert.True(File.Exists(dbPath));

        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='time_entries'";
        var tableCount = Convert.ToInt32(command.ExecuteScalar());

        Assert.Equal(1, tableCount);
    }

    [Fact]
    public void Constructor_CreatesMissingDirectoryStructure()
    {
        var root = Path.Combine(Path.GetTempPath(), $"db_root_{Guid.NewGuid()}");
        var nested = Path.Combine(root, "child", "grandchild");
        var dbPath = Path.Combine(nested, "tracker.db");

        _tempDirectories.Add(root);
        _tempFiles.Add(dbPath);

        _ = new Database(dbPath);

        Assert.True(Directory.Exists(nested));
        Assert.True(File.Exists(dbPath));
    }

    [Fact]
    public void Constructor_IsIdempotent_WhenCalledMultipleTimes()
    {
        var dbPath = CreateTempDatabasePath();

        _ = new Database(dbPath);
        _ = new Database(dbPath);

        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='time_entries'";
        var tableCount = Convert.ToInt32(command.ExecuteScalar());

        Assert.Equal(1, tableCount);
    }

    [Fact]
    public void Constructor_SupportsInMemoryDatabase()
    {
        var database = new Database(":memory:");

        Assert.NotNull(database);
    }
}
