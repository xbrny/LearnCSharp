using Microsoft.Data.Sqlite;

namespace CodingTracker;

public class HabitDb
{
    private static SqliteConnection _connection = null!;

    public void Bootstrap()
    {
        var dataSourcePath = Path.Combine(@"..\..\..\MyHabitLoggerDB.db");

        _connection = CreateDbConnection(dataSourcePath);

        _connection.Open();
    }

    public void CreateDatabaseAndTables()
    {
        var command = _connection.CreateCommand();

        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS 'Entries' (
                Id INTEGER PRIMARY KEY,
                StartDate TEXT,
                EndDate TEXT,
                Remark TEXT
            );
            ";

        command.ExecuteNonQuery();
    }

    public void CloseConnection()
    {
        _connection.Close();
    }

    private SqliteConnection CreateDbConnection(string dataSource)
    {
        return new SqliteConnection($"Data Source={dataSource}");
    }

    public void CreateEntry(HabitEntry entry)
    {
        var command = _connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO 'Entries' (
                StartDate,
                EndDate,
                Remark
            ) VALUES (
                $startDate,
                $endDate,
                $remark
            );
            ";

        command.Parameters.AddWithValue("$startDate", entry.StartDate);
        command.Parameters.AddWithValue("$endDate", entry.EndDate);
        command.Parameters.AddWithValue("$remark", entry.Remark);

        command.ExecuteNonQuery();
    }

    public List<HabitEntry> GetAllEntries()
    {
        List<HabitEntry> entries = new();

        var command = _connection.CreateCommand();

        command.CommandText = @"
SELECT Id, StartDate, EndDate, Remark FROM 'Entries';
";
        using var reader = command.ExecuteReader();

        if (!reader.HasRows)
        {
            return entries;
        }

        while (reader.Read())
        {
            var id = reader.GetInt32(0);
            var startDate = reader.GetDateTime(1);
            var endDate = reader.GetDateTime(2);
            var remark = reader.GetString(3);

            entries.Add(new HabitEntry()
            {
                Id = id,
                StartDate = startDate,
                EndDate = endDate,
                Remark = remark,
            });
        }

        reader.Close();


        return entries;
    }

    public void UpdateEntry(int id, HabitEntry entry)
    {
        var command = _connection.CreateCommand();

        command.CommandText = @"
            UPDATE 'Entries' SET 
                StartDate = $startDate,
                EndDate = $endDate,
                Remark = $remark
            WHERE Id = $id;
            ";

        command.Parameters.AddWithValue("$startDate", entry.StartDate);
        command.Parameters.AddWithValue("$endDate", entry.EndDate);
        command.Parameters.AddWithValue("$remark", entry.Remark);
        command.Parameters.AddWithValue("$id", id);
        
        command.ExecuteNonQuery();
    }

    public void DeleteEntry(int id)
    {
        var command = _connection.CreateCommand();

        command.CommandText = @"
            DELETE FROM 'Entries' WHERE Id = $id;
            ";

        command.Parameters.AddWithValue("$id", id);

        command.ExecuteNonQuery();
    }

    public void DeleteAllEntries()
    {
        var command = _connection.CreateCommand();

        command.CommandText = @"
            DELETE FROM 'Entries';
            ";

        command.ExecuteNonQuery();
    }
}