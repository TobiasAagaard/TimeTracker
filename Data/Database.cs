using Microsoft.Data.Sqlite;

namespace TimeTracker.Data;

public class Database
{
    private readonly string _databasePath;

    public Database(string databasePath)
    {
        _databasePath = databasePath;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {

         var directory = Path.GetDirectoryName(_databasePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
        using var connection = new SqliteConnection($"Data Source={_databasePath}");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS time_entries (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                start_time TEXT NOT NULL,
                end_time TEXT NOT NULL
            );";

        command.ExecuteNonQuery();
    }
}