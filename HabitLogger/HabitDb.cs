using Microsoft.Data.Sqlite;

namespace HabitLogger;

public static class HabitDb
{
    private static SqliteConnection _connection = null!;

    public static async Task BootstrapDb()
    {
        var dataSourcePath = Path.Combine(@"..\..\..\MyHabitLoggerDB.db");

        _connection = CreateDbConnection(dataSourcePath);

        _connection.Open();

        await CreateTables();
    }

    public static void CloseConnection()
    {
        _connection?.Close();
    }

    private static SqliteConnection CreateDbConnection(string dataSource)
    {
        return new SqliteConnection($"Data Source={dataSource}");
    }

    private static async Task CreateTables()
    {
        var command = _connection.CreateCommand();

        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS 'Habits' (
                Id INTEGER PRIMARY KEY,
                Name TEXT,
                CreatedAt TEXT,
                Type TEXT
            );

            CREATE TABLE IF NOT EXISTS 'Entries' (
                Id INTEGER PRIMARY KEY,
                HappenedOn TEXT,
                Value REAL,
                HabitId INTEGER
            );
            ";

        await command.ExecuteNonQueryAsync();
    }

    public static async Task ClearAllDataAction()
    {
        var command = _connection.CreateCommand();

        command.CommandText = @"
            DELETE FROM 'Habits';
            ";

        await command.ExecuteNonQueryAsync();
    }

    public static async Task AddHabitAction(string name, string type)
    {
        var command = _connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO 'Habits' (
                Name,
                CreatedAt,
                Type
            ) VALUES (
                $name,
                $createdAt,
                $type
            );
            ";

        command.Parameters.AddWithValue("$name", name);
        command.Parameters.AddWithValue("$createdAt", DateTime.Now);
        command.Parameters.AddWithValue("$type", type);

        await command.ExecuteNonQueryAsync();
    }

    public static async Task UpdateHabitAction(int id, string name, string type)
    {
        var command = _connection.CreateCommand();

        command.CommandText = @"
            UPDATE 'Habits' SET Name = $name,Type = $type
            WHERE Id = $id
            ";

        command.Parameters.AddWithValue("$name", name);
        command.Parameters.AddWithValue("$type", type);
        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
    }

    public static async Task<List<Habit>> GetAllHabitsAction()
    {
        List<Habit> habitList = new();

        var command = _connection.CreateCommand();

        command.CommandText = @"
SELECT id, name, createdAt, type FROM 'Habits';
";
        await using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
        {
            return habitList;
        }

        while (reader.Read())
        {
            var id = reader.GetInt32(0);
            var name = reader.GetString(1);
            var createdAt = reader.GetDateTime(2);
            var measurement = reader.GetString(3);

            Measurement measurementEnum;

            Enum.TryParse(measurement, true, out measurementEnum);

            habitList.Add(new Habit(id, name, createdAt, measurementEnum));
        }

        reader.Close();

        return habitList;
    }

    public static async Task DeleteHabitAction(int id)
    {
        var command = _connection.CreateCommand();

        command.CommandText = @"
            DELETE FROM 'Habits' WHERE Id = $id;
            ";

        command.Parameters.AddWithValue("$id", id);

        await command.ExecuteNonQueryAsync();
    }

    public static async Task AddEntryToHabit(int habitId, double value)
    {
        var command = _connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO 'Entries' (
                Value,
                HappenedOn,
                HabitId
            ) VALUES (
                $value,
                $happenedOn,
                $habitId
            );
            ";

        command.Parameters.AddWithValue("value", value);
        command.Parameters.AddWithValue("happenedOn", DateTime.Now);
        command.Parameters.AddWithValue("habitId", habitId);

        await command.ExecuteNonQueryAsync();
    }
}