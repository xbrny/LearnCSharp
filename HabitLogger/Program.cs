using System.Text.RegularExpressions;

namespace HabitLogger;

internal static class Program
{
    private static bool _endProgram = false;

    public static async Task Main(string[] args)
    {
        await HabitDb.BootstrapDb();

        while (!_endProgram)
        {
            await RunApp();
        }

        HabitDb.CloseConnection();
    }

    private static async Task RunApp()
    {
        PrintHeader("My Habits");

        await ShowHabitList();

        Console.WriteLine();

        PrintHeader("Main Menu");
        Console.WriteLine("l - View or log habit entry");
        Console.WriteLine("a - Add new habit");
        Console.WriteLine("d - Delete habit");
        Console.WriteLine("u - Update habit");
        Console.WriteLine("r - Clear all data (Danger!)");
        Console.WriteLine("0 - Exit application");
        Console.Write("\nEnter your selection: ");
        var selectedMenu = Console.ReadLine() ?? "";
        while (!Regex.IsMatch(selectedMenu, "(v|a|d|u|r|0)"))
        {
            Console.Write("Please enter a valid option from the menu: ");
            selectedMenu = Console.ReadLine() ?? "";
        }

        if (selectedMenu == "0")
        {
            _endProgram = true;
        }

        Console.Clear();

        switch (selectedMenu)
        {
            case "l":
                await ViewOrLogHabitEntry();
                break;
            case "a":
                await AddHabit();
                break;
            case "d":
                await DeleteHabit();
                break;
            case "u":
                await UpdateHabit();
                break;
            case "r":
                await ClearAllData();
                break;
        }

        Console.Clear();
    }

    private static async Task DeleteHabit()
    {
        PrintHeader("Delete Habit");

        var list = await ShowHabitList();

        if (list.Count < 1)
        {
            ShowPressAnyKeyToContinue();
            return;
        }

        var (selectedHabit, id) = PromptInputToSelectHabit(list);

        if (selectedHabit == null)
        {
            Console.WriteLine($"\nCannot find the habit");
            ShowPressAnyKeyToContinue();
            return;
        }

        await HabitDb.DeleteHabitAction(selectedHabit.Id);

        Console.WriteLine($"\nHabit with id {selectedHabit.Id} was deleted");

        ShowPressAnyKeyToContinue();
    }

    private static async Task ViewOrLogHabitEntry()
    {
        var exit = false;

        while (!exit)
        {
            PrintHeader("Log habit entry");

            var list = await ShowHabitList();

            if (list.Count < 1)
            {
                ShowPressAnyKeyToContinue();
                break;
            }

            var (selectedHabit, id) = PromptInputToSelectHabit(list);

            if (selectedHabit == null)
            {
                Console.WriteLine($"\nCannot find the habit");
                ShowPressAnyKeyToContinue();
                continue;
            }

            Console.WriteLine($"\nLog an entry ({selectedHabit.Type}): ");
            var spendInput = Console.ReadLine() ?? "";
            double spend;
            while (double.TryParse(spendInput, out spend))
            {
                Console.WriteLine("This field is required, please re-enter a valid input");
                spendInput = Console.ReadLine() ?? "";
            }

            await HabitDb.AddEntryToHabit(selectedHabit.Id, spend);

            Console.WriteLine("\nEntry added, Good job!");

            ShowPressAnyKeyToContinue();
        }
    }

    private static async Task<List<Habit>> ShowHabitList()
    {
        var habitList = await HabitDb.GetAllHabitsAction();

        if (habitList.Count < 1)
        {
            Console.WriteLine("You don't have any habit yet");
        }
        else
        {
            foreach (var habit in habitList)
            {
                Console.WriteLine(habit);
            }
        }

        return habitList;
    }

    private static void ShowPressAnyKeyToContinue()
    {
        Console.WriteLine("\nPlease any key to continue");
        Console.ReadKey();
    }

    private static async Task AddHabit()
    {
        PrintHeader("Add New Habit");

        Console.WriteLine("\nWhat is the name of this new habit?");
        var name = Console.ReadLine() ?? "";
        while (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("This field is required, please re-enter a valid input");
            name = Console.ReadLine() ?? "";
        }

        Console.WriteLine("\nHow are you going to track this habit (time, quantity)?");
        var type = Console.ReadLine() ?? "";
        while (!Regex.IsMatch(type, "(time|quantity)"))
        {
            Console.WriteLine("This field only accept time or quantity, please re-enter a valid input");
            type = Console.ReadLine() ?? "";
        }

        Console.WriteLine("\nCreating, please wait...");

        await HabitDb.AddHabitAction(name, type);

        Console.WriteLine("\nNew habit added, hooray!");

        ShowPressAnyKeyToContinue();
    }


    private static async Task UpdateHabit()
    {
        PrintHeader("Update Habit");

        var list = await ShowHabitList();

        if (list.Count < 1)
        {
            ShowPressAnyKeyToContinue();
            return;
        }

        var (selectedHabit, id) = PromptInputToSelectHabit(list);

        if (selectedHabit == null)
        {
            Console.WriteLine($"\nCannot find the habit");
            ShowPressAnyKeyToContinue();
            return;
        }

        Console.WriteLine("\nLeave the input empty if you don't want to update the field\n");

        Console.WriteLine($"Update the name [\"{selectedHabit.Name}\"]: ");
        var name = Console.ReadLine();

        Console.WriteLine($"\nUpdate the measurement (time, quantity) [\"{selectedHabit.Type}\"]: ");
        var type = Console.ReadLine() ?? "";
        while (!Regex.IsMatch(type, "(time|quantity)?"))
        {
            Console.WriteLine("This field only accept time or quantity, please re-enter a valid input");
            type = Console.ReadLine();
        }

        Console.WriteLine("\nUpdate, please wait...");

        var nameWithDefault = string.IsNullOrWhiteSpace(name) ? selectedHabit.Name : name;
        var typeWithDefault = string.IsNullOrWhiteSpace(type) ? selectedHabit.Type.ToString() : type;

        await HabitDb.UpdateHabitAction(selectedHabit.Id, nameWithDefault, typeWithDefault);

        Console.WriteLine("\nHabit updated");

        ShowPressAnyKeyToContinue();
    }

    private static async Task ClearAllData()
    {
        Console.Write("This action is irreversible! enter 'y' to continue or 'n' to go back: ");
        var input = Console.ReadLine() ?? "";
        if (!Regex.IsMatch(input, "(y|n)"))
        {
            Console.Write("Invalid input, please enter your selection again: ");
            input = Console.ReadLine() ?? "";
        }

        if (input.Equals("n", StringComparison.CurrentCultureIgnoreCase))
        {
            return;
        }

        await HabitDb.ClearAllDataAction();

        Console.WriteLine("\nAll data were cleared");

        ShowPressAnyKeyToContinue();
    }

    private static (Habit? habit, int input) PromptInputToSelectHabit(IEnumerable<Habit> list)
    {
        Console.Write("\nEnter the habit id to select: ");
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

    private static void PrintHeader(string title)
    {
        Console.WriteLine($"==== {title} ====");
    }
}