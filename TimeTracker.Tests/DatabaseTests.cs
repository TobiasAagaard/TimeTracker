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
        // Clean up test files and directories
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

    private string GetTempDatabasePath()
    {
        var path = Path.Combine(Path.GetTempPath(), $"test_db_{Guid.NewGuid()}.db");
        _tempFiles.Add(path);
        return path;
    }

    private string GetTempDatabasePathWithDirectory()
    {
        var dirPath = Path.Combine(Path.GetTempPath(), $"test_dir_{Guid.NewGuid()}");
        _tempDirectories.Add(dirPath);
        var dbPath = Path.Combine(dirPath, "test.db");
        _tempFiles.Add(dbPath);
        return dbPath;
    }

    [Fact]
    public void Constructor_InitializesDatabase_Successfully()
    {
        // Arrange
        var dbPath = GetTempDatabasePath();

        // Act
        var database = new Database(dbPath);

        // Assert
        Assert.True(File.Exists(dbPath));
    }

    [Fact]
    public void Constructor_CreatesDirectory_WhenDirectoryDoesNotExist()
    {
        // Arrange
        var dbPath = GetTempDatabasePathWithDirectory();
        var directory = Path.GetDirectoryName(dbPath);

        Assert.False(Directory.Exists(directory));

        // Act
        var database = new Database(dbPath);

        // Assert
        Assert.True(Directory.Exists(directory));
        Assert.True(File.Exists(dbPath));
    }

    [Fact]
    public void Constructor_CreatesTimeEntriesTable_Successfully()
    {
        // Arrange
        var dbPath = GetTempDatabasePath();

        // Act
        var database = new Database(dbPath);

        // Assert - Verify table exists by querying it
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='time_entries'";
        var result = command.ExecuteScalar();

        Assert.NotNull(result);
        Assert.Equal("time_entries", result);
    }

    [Fact]
    public void Constructor_CreatesTableWithCorrectSchema()
    {
        // Arrange
        var dbPath = GetTempDatabasePath();

        // Act
        var database = new Database(dbPath);

        // Assert - Verify table schema
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA table_info(time_entries)";
        using var reader = command.ExecuteReader();

        var columns = new List<(string name, string type, bool notNull)>();
        while (reader.Read())
        {
            columns.Add((
                reader.GetString(1), // name
                reader.GetString(2), // type
                reader.GetInt32(3) == 1 // notnull
            ));
        }

        Assert.Equal(3, columns.Count);
        Assert.Contains(columns, c => c.name == "id" && c.type == "INTEGER");
        Assert.Contains(columns, c => c.name == "start_time" && c.type == "TEXT" && c.notNull);
        Assert.Contains(columns, c => c.name == "end_time" && c.type == "TEXT" && c.notNull);
    }

    [Fact]
    public void Constructor_IsIdempotent_CanBeCalledMultipleTimes()
    {
        // Arrange
        var dbPath = GetTempDatabasePath();

        // Act - Initialize database twice
        var database1 = new Database(dbPath);
        var database2 = new Database(dbPath);

        // Assert - Should not throw and table should still exist
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='time_entries'";
        var result = command.ExecuteScalar();

        Assert.NotNull(result);
    }

    [Fact]
    public void Constructor_HandlesInMemoryDatabase()
    {
        // Arrange & Act
        var database = new Database(":memory:");

        // Assert - Should not throw
        Assert.NotNull(database);
    }

    [Fact]
    public void Constructor_HandlesPathWithoutDirectory()
    {
        // Arrange
        var dbPath = "simple.db";
        _tempFiles.Add(dbPath);

        // Act
        var database = new Database(dbPath);

        // Assert
        Assert.True(File.Exists(dbPath));
    }

    [Fact]
    public void Constructor_CreatesNestedDirectories()
    {
        // Arrange
        var dirPath = Path.Combine(Path.GetTempPath(), $"test_parent_{Guid.NewGuid()}", "child", "grandchild");
        _tempDirectories.Add(Path.Combine(Path.GetTempPath(), $"test_parent_{Guid.NewGuid().ToString().Substring(0, 8)}"));
        var dbPath = Path.Combine(dirPath, "test.db");
        _tempFiles.Add(dbPath);

        // Act
        var database = new Database(dbPath);

        // Assert
        Assert.True(Directory.Exists(dirPath));
        Assert.True(File.Exists(dbPath));
    }

    [Fact]
    public void Constructor_AllowsConnectionAfterInitialization()
    {
        // Arrange
        var dbPath = GetTempDatabasePath();
        var database = new Database(dbPath);

        // Act & Assert - Should be able to open connection and insert data
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO time_entries (start_time, end_time) VALUES ('2026-01-01', '2026-01-02')";
        var rowsAffected = command.ExecuteNonQuery();

        Assert.Equal(1, rowsAffected);
    }

    [Fact]
    public void Constructor_WithEmptyPath_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Database(""));
    }

    [Fact]
    public void Constructor_WithNullPath_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Database(null!));
    }
}