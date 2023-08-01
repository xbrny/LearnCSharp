using System.Text.RegularExpressions;

namespace BasicCalculator;

static class Calculator
{
    public static List<CalculationHistory> CalculationHistory = new();

    public static double DoOperation(double number1, double number2, string operation)
    {
        var result = double.NaN;

        switch (operation)
        {
            case "a":
                result = number1 + number2;
                break;
            case "s":
                result = number1 - number2;
                break;
            case "m":
                result = number1 * number2;
                break;
            case "d":
                if (number2 != 0)
                {
                    result = number1 / number2;
                }

                break;
        }

        return result;
    }

    public static string GetOperationSymbol(string operation)
    {
        return operation switch
        {
            "a" => "+",
            "s" => "-",
            "m" => "*",
            "d" => "/",
            _ => "unknown"
        };
    }

    public static void PrintResult(double cleanNumber1, double cleanNumber2, string operation)
    {
        try
        {
            var result = DoOperation(cleanNumber1, cleanNumber2, operation);

            if (double.IsNaN(result))
            {
                Console.WriteLine("This operation will result in mathematical error");
            }
            else
            {
                var historyEntry = new CalculationHistory(cleanNumber1, operation, cleanNumber2);
                CalculationHistory.Add(historyEntry);
                Console.WriteLine($"{cleanNumber1} {GetOperationSymbol(operation)} {cleanNumber2} = {result:0.####}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Oh no! An exception occurred trying to do the math.\n - Details: {0}", e.Message);
            throw;
        }
    }

    public static void PrintHistory()
    {
        Console.WriteLine("History: ");
        foreach (var (history, index) in CalculationHistory.Select((history, i) => (history, i)))
        {
            Console.WriteLine(
                $"{index}: {history.Number1} {Calculator.GetOperationSymbol(history.Operation)} {history.Number2}");
        }
    }
}

record CalculationHistory(double Number1, string Operation, double Number2);

class Program
{
    public static void Main(string[] args)
    {
        var endProgram = false;
        var sessionCount = 0;
        var calculationHistory = Calculator.CalculationHistory;

        while (!endProgram)
        {
            sessionCount++;


            Console.WriteLine($"Session {sessionCount}");

            var hasHistory = Calculator.CalculationHistory.Count > 0;
            var selectedMenu = "n";

            if (hasHistory)
            {
                Console.WriteLine("View history (h) or start a new calculation (n): ");
                selectedMenu = Console.ReadLine() ?? "";
            }

            while (!Regex.IsMatch(selectedMenu, "(h|n)", RegexOptions.IgnoreCase))
            {
                Console.Write("Please enter a valid option: ");
                selectedMenu = Console.ReadLine() ?? "";
            }

            if (selectedMenu.Equals("h", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.Clear();

                Calculator.PrintHistory();

                Console.WriteLine("Perform an operation to the history above:");
                Console.WriteLine("\t d - Delete");
                Console.WriteLine("\t c - Calculate");
                Console.WriteLine("\t b - Back");
                Console.Write("Enter your selection and press enter: ");
                var historyOperation = Console.ReadLine() ?? "";
                while (!Regex.IsMatch(historyOperation, "(d|c|b)", RegexOptions.IgnoreCase))
                {
                    Console.Write("Please enter a valid option: ");
                    historyOperation = Console.ReadLine() ?? "";
                }

                if (historyOperation.Equals("b", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.Clear();
                    continue;
                }

                Console.Clear();

                Calculator.PrintHistory();

                Console.Write("Enter the number at most left of the row you want to perform this operation: ");
                var selectedIndex = Console.ReadLine() ?? "";
                int cleanSelectedIndex;
                while (!int.TryParse(selectedIndex, out cleanSelectedIndex))
                {
                    selectedIndex = Console.ReadLine() ?? "";
                }


                if (cleanSelectedIndex >= calculationHistory.Count)
                {
                    Console.WriteLine(
                        $"History with index {cleanSelectedIndex} was not found, press any key to continue");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }

                var selectedHistory = calculationHistory[cleanSelectedIndex];

                switch (historyOperation)
                {
                    case "d":
                        calculationHistory.RemoveAt(cleanSelectedIndex);
                        Console.WriteLine($"Deleted item at index {cleanSelectedIndex}");
                        break;
                    case "c":
                        Calculator.PrintResult(selectedHistory.Number1, selectedHistory.Number2,
                            selectedHistory.Operation);
                        break;
                }
            }
            else
            {
                Console.Clear();

                double cleanNumber1;
                double cleanNumber2;

                Console.Write("Enter the first number: ");
                var number1 = Console.ReadLine();

                while (!double.TryParse(number1, out cleanNumber1))
                {
                    Console.Write("Please enter a valid number: ");
                    number1 = Console.ReadLine();
                }

                Console.Write("Enter the second number: ");
                var number2 = Console.ReadLine();

                while (!double.TryParse(number2, out cleanNumber2))
                {
                    Console.Write("Please enter a valid number: ");
                    number2 = Console.ReadLine();
                }

                Console.WriteLine("Select an operation");
                Console.WriteLine("\t a - Add");
                Console.WriteLine("\t s - Subtract");
                Console.WriteLine("\t m - Multiply");
                Console.WriteLine("\t d - Divide");
                Console.Write("Enter your selection and press enter: ");

                var operation = Console.ReadLine() ?? "";
                while (!Regex.IsMatch(operation, "(a|s|m|d)", RegexOptions.IgnoreCase))
                {
                    Console.Write("Please enter a valid operation: ");
                    operation = Console.ReadLine() ?? "";
                }

                Calculator.PrintResult(cleanNumber1, cleanNumber2, operation);
            }

            Console.WriteLine("\n");

            Console.Write("Please 'n' to exit the game or press any key to start a new game: ");
            endProgram = Console.ReadLine() == "n";
            Console.Clear();
        }
    }
}