namespace CodingTracker;

internal abstract class Program
{
    public static void Main(string[] args)
    {
        var habitView = new HabitView();

        habitView.ShowAppTitle();

        var habitDb = new HabitDb();

        habitDb.Bootstrap();
        habitDb.CreateDatabaseAndTables();

        var habitController = new HabitController(habitView: habitView, habitDb: habitDb);

        habitController.Run();

        habitDb.CloseConnection();
    }
}