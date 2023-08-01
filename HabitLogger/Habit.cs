public record Habit(int Id, String Name, DateTime CreatedAt, Measurement Type);

public record Entry(int Id, DateTime HappenedAt, double Value, string HabitId);

public enum Measurement
{
    Time,
    Quantity,
}