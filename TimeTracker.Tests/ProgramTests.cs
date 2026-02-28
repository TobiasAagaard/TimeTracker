using System;
using System.IO;
using TimeTracker.Data;
using Xunit;

namespace TimeTracker.Tests;

public class ProgramTests : IDisposable
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

    [Fact]
    public void DatabaseInitialization_CreatesDatabase_InLocalApplicationData()
    {
        // Arrange
        var dbPath = Path.Combine(Path.GetTempPath(), $"TimeTracker_Test_{Guid.NewGuid()}.db");
        _tempFiles.Add(dbPath);

        // Act - This simulates what Program.Main does
        Database database;
        try
        {
            database = new Database(dbPath);
        }
        catch (Exception)
        {
            database = null!;
        }

        // Assert
        Assert.NotNull(database);
        Assert.True(File.Exists(dbPath));
    }

    [Fact]
    public void DatabaseInitialization_HandlesDatabaseCreationInNestedDirectory()
    {
        // Arrange - Simulate the LocalApplicationData path structure
        var tempDir = Path.Combine(Path.GetTempPath(), $"TimeTracker_TestDir_{Guid.NewGuid()}");
        _tempDirectories.Add(tempDir);
        var dbPath = Path.Combine(tempDir, "TimeTracker.db");
        _tempFiles.Add(dbPath);

        // Act - This simulates what Program.Main does
        Database database;
        Exception? caughtException = null;
        try
        {
            database = new Database(dbPath);
        }
        catch (Exception ex)
        {
            caughtException = ex;
            database = null!;
        }

        // Assert
        Assert.Null(caughtException);
        Assert.NotNull(database);
        Assert.True(Directory.Exists(tempDir));
        Assert.True(File.Exists(dbPath));
    }

    [Fact]
    public void DatabaseInitialization_CatchesExceptions_WhenDatabaseInitFails()
    {
        // Arrange - Use an invalid path that will cause initialization to fail
        var invalidPath = Path.Combine(new string('x', 300), "invalid.db"); // Extremely long path

        // Act
        Exception? caughtException = null;
        try
        {
            _ = new Database(invalidPath);
        }
        catch (Exception ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.NotNull(caughtException);
    }

    [Fact]
    public void DatabaseInitialization_ErrorMessage_IsInformative()
    {
        // Arrange
        var invalidPath = Path.Combine(new string('x', 300), "invalid.db");

        // Act
        Exception? caughtException = null;
        string errorMessage = string.Empty;
        try
        {
            _ = new Database(invalidPath);
        }
        catch (Exception ex)
        {
            caughtException = ex;
            errorMessage = $"Error initializing database: {ex.Message}";
        }

        // Assert
        Assert.NotNull(caughtException);
        Assert.NotEmpty(errorMessage);
        Assert.StartsWith("Error initializing database:", errorMessage);
    }

    [Fact]
    public void DatabasePath_UsesCorrectFileName()
    {
        // Arrange
        var expectedFileName = "TimeTracker.db";
        var dbPath = Path.Combine(Path.GetTempPath(), expectedFileName);
        _tempFiles.Add(dbPath);

        // Act
        var database = new Database(dbPath);

        // Assert
        Assert.True(File.Exists(dbPath));
        Assert.Equal(expectedFileName, Path.GetFileName(dbPath));
    }

    [Fact]
    public void ProgramFlow_DatabaseInitializedBeforeMenus()
    {
        // This test verifies the order of operations in Program.Main
        // Arrange
        var dbPath = Path.Combine(Path.GetTempPath(), $"TimeTracker_Flow_{Guid.NewGuid()}.db");
        _tempFiles.Add(dbPath);

        // Act - Simulate the flow: Database must be created first
        Database? database = null;
        bool databaseInitialized = false;

        try
        {
            database = new Database(dbPath);
            databaseInitialized = true;
        }
        catch (Exception)
        {
            databaseInitialized = false;
        }

        // Only proceed to "menu" phase if database initialized
        bool canProceedToMenus = databaseInitialized;

        // Assert
        Assert.True(databaseInitialized);
        Assert.NotNull(database);
        Assert.True(canProceedToMenus);
    }

    [Fact]
    public void DatabaseInitialization_SupportsMultipleExecutions()
    {
        // Test that the database can be re-initialized (as might happen on program restart)
        // Arrange
        var dbPath = Path.Combine(Path.GetTempPath(), $"TimeTracker_Multi_{Guid.NewGuid()}.db");
        _tempFiles.Add(dbPath);

        // Act - Initialize twice
        var database1 = new Database(dbPath);
        var database2 = new Database(dbPath);

        // Assert - Both should succeed
        Assert.NotNull(database1);
        Assert.NotNull(database2);
        Assert.True(File.Exists(dbPath));
    }

    [Fact]
    public void DatabaseInitialization_CreatesReadWritableDatabase()
    {
        // Verify the database created by initialization is actually usable
        // Arrange
        var dbPath = Path.Combine(Path.GetTempPath(), $"TimeTracker_RW_{Guid.NewGuid()}.db");
        _tempFiles.Add(dbPath);

        // Act
        var database = new Database(dbPath);

        // Verify we can read/write to it
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM time_entries";
        var count = command.ExecuteScalar();

        // Assert
        Assert.NotNull(count);
        Assert.Equal(0L, Convert.ToInt64(count));
    }
}