namespace CodingTracker;

public static class Utils
{
    public static void PrintDivider()
    {
        Console.WriteLine("---------------------------------------");
    }

    public static string[] ToStringArray(Array? array)
    {
        if (array == null || array.Length < 1)
        {
            return Array.Empty<string>();
        }

        var newArray = new string[array.Length];

        for (var i = 0; i < array.Length; i++)
        {
            newArray[i] = array.GetValue(i)?.ToString() ?? string.Empty;
        }

        return newArray;
    }
}