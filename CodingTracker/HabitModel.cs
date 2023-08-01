namespace CodingTracker;

public record HabitEntry
{
    public int Id;
    public DateTime StartDate;
    public DateTime EndDate;
    public string? Remark;

    public string CalculateDuration()
    {
        return $"{(EndDate - StartDate).TotalHours:0.##} hours";
    }
}