using ConsoleTableExt;

namespace CodingTracker;

public enum MenuOption
{
    ViewAll = 1,
    LogEntry = 2,
    DeleteEntry = 3,
    UpdateEntry = 4,
    Exit = 0,
}

public class HabitView
{
    public MenuOption ShowMenu()
    {
        Utils.PrintDivider();
        Console.WriteLine("Main Menu");

        var menuOptions = Enum.GetValuesAsUnderlyingType(typeof(MenuOption));
        foreach (int menuItem in menuOptions)
        {
            var item = (MenuOption)menuItem;
            Console.WriteLine(item.MenuOptionLabel());
        }

        var menuString = Console.ReadLine() ?? "";

        var menuOptionsString = Utils.ToStringArray(menuOptions);

        while (Array.IndexOf(menuOptionsString, menuString) < 0)
        {
            Console.WriteLine("Please enter a valid input");
            menuString = Console.ReadLine() ?? "";
        }

        Console.Clear();

        return (MenuOption)int.Parse(menuString);
    }

    public HabitEntry ShowCreateEntryForm(HabitEntry? entryToUpdate = null)
    {
        bool isUpdate = entryToUpdate != null;

        Utils.PrintDivider();
        Console.WriteLine($"{(isUpdate ? "Update" : "New")} entry");
        Utils.PrintDivider();
        Console.WriteLine();

        var format = "dd-MM-yyyy HH:mm";

        var defaultStartDate = isUpdate ? entryToUpdate?.StartDate.ToString(format) : DateTime.Now.ToString(format);
        var defaultEndDate = isUpdate ? entryToUpdate?.EndDate.ToString(format) : null;
        var defaultRemark = isUpdate ? entryToUpdate?.Remark : null;

        Console.Write($"Enter start date & time in {format} format ({defaultStartDate}): ");
        var startDateTimeRaw = Console.ReadLine();

        startDateTimeRaw = string.IsNullOrWhiteSpace(startDateTimeRaw) ? defaultStartDate : startDateTimeRaw;

        DateTime startDateTime;
        while (!DateTime.TryParseExact(startDateTimeRaw, format, provider: null, style: 0, out startDateTime))
        {
            Console.WriteLine($"Invalid format, please enter value in {format} format");
            startDateTimeRaw = Console.ReadLine();
        }

        Console.Write(
            $"Enter end date & time in {format} format {(isUpdate ? $"({defaultEndDate})" : "")}: ");
        var endDateTimeRaw = Console.ReadLine();

        if (isUpdate)
        {
            endDateTimeRaw = string.IsNullOrWhiteSpace(endDateTimeRaw) ? defaultEndDate : endDateTimeRaw;
        }

        DateTime endDateTime;
        while (!DateTime.TryParseExact(endDateTimeRaw, format, provider: null, style: 0, out endDateTime))
        {
            Console.WriteLine($"Invalid format, please enter value in {format} format");
            endDateTimeRaw = Console.ReadLine();
        }

        Console.Write($"Enter remark {(isUpdate ? $"({defaultRemark})" : "(optional)")}: ");
        var remark = Console.ReadLine();

        if (isUpdate)
        {
            remark = string.IsNullOrWhiteSpace(remark) ? defaultRemark : remark;
        }

        var newEntry = new HabitEntry()
        {
            StartDate = startDateTime,
            EndDate = endDateTime,
            Remark = remark,
        };

        if (entryToUpdate != null)
        {
            newEntry = newEntry with { Id = entryToUpdate.Id };
        }

        return newEntry;
    }

    public void ShowAllEntries(List<HabitEntry> entries)
    {
        Utils.PrintDivider();
        Console.WriteLine("All Entries");
        Utils.PrintDivider();
        Console.WriteLine();

        PrintAllEntries(entries);

        Console.WriteLine("\nPlease any key to continue");
        Console.ReadKey();
    }

    public HabitEntry? PromptDeleteEntry(List<HabitEntry> entries)
    {
        Utils.PrintDivider();
        Console.WriteLine("Delete Entry");
        Utils.PrintDivider();
        Console.WriteLine();

        PrintAllEntries(entries);

        var (selectedEntry, id) = PromptInputToSelectEntry(entries);

        if (selectedEntry == null)
        {
            Console.WriteLine($"\nCannot find entry with Id {id}");
        }

        return selectedEntry;
    }

    public HabitEntry? PromptUpdateEntry(List<HabitEntry> entries)
    {
        Utils.PrintDivider();
        Console.WriteLine("Update Entry");
        Utils.PrintDivider();
        Console.WriteLine();

        PrintAllEntries(entries);

        var (selectedEntry, id) = PromptInputToSelectEntry(entries);

        if (selectedEntry == null)
        {
            Console.WriteLine($"\nCannot find entry with Id {id}");

            return selectedEntry;
        }

        return ShowCreateEntryForm(selectedEntry);
    }

    private void PrintAllEntries(List<HabitEntry> entries)
    {
        if (entries.Count < 1)
        {
            Console.WriteLine("You don't have an entry yet");
            return;
        }

        var tableData = new List<List<object>>();

        foreach (var entry in entries)
        {
            var tableRow = new List<object?>();

            foreach (var prop in entry.GetType().GetFields())
            {
                object? value = prop.GetValue(entry);
                
                if (value is DateTime)
                {
                    value = $"{value:yyyy-M-d HH:mm}";
                }
                
                Console.Write(prop.Name);

                if (prop.Name.Equals("Remark", StringComparison.CurrentCultureIgnoreCase))
                {
                    tableRow.Add(entry.CalculateDuration());
                }

                tableRow.Add(value);
            }

            tableData.Add(tableRow!);
        }

        ConsoleTableBuilder
            .From(tableData)
            .WithFormat(ConsoleTableBuilderFormat.Alternative)
            .WithColumn("Id", "Start date", "End date", "Duration", "Remark")
            .ExportAndWriteLine();
    }

    private (HabitEntry? entry, int input) PromptInputToSelectEntry(IEnumerable<HabitEntry> list)
    {
        Console.Write("\nEnter the id to select: ");
        var idInput = Console.ReadLine() ?? "";
        int id;
        while (!int.TryParse(idInput, out id))
        {
            Console.Write($"Please enter a valid input");
            idInput = Console.ReadLine() ?? "";
        }

        var selectedHabit = list.FirstOrDefault(habit => habit.Id == id);

        return (selectedHabit, id);
    }

    public void ShowAppTitle()
    {
        Utils.PrintDivider();
        Console.WriteLine("Welcome to Coding Logger!");
        Utils.PrintDivider();
    }

    public void ShowSuccessfulMessage()
    {
        Console.WriteLine("\nOperation complete, please any key to continue");
        Console.ReadKey();
    }

    public void ShowErrorMessage(Exception e)
    {
        Console.WriteLine("\nThe requested operation cannot be completed\nDetails: {0}", e.Message);
        Console.WriteLine("\nPlease any key to continue");
        Console.ReadKey();
    }
}

public static class HabitViewExtension
{
    public static string MenuOptionLabel(this MenuOption option)
    {
        return option switch
        {
            MenuOption.ViewAll => "Press 1 to view all entries",
            MenuOption.LogEntry => "Press 2 to log new entry",
            MenuOption.DeleteEntry => "Press 3 to delete entry",
            MenuOption.UpdateEntry => "Press 4 to update entry",
            MenuOption.Exit => "Press 0 to exit application",
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };
    }
}