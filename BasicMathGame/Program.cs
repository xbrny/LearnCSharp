/* # Math game [https://www.thecsharpacademy.com/project/53]
 * ## Requirements
 *  [-] You need to create a Math game containing the 4 basic operations
 *  [-] The divisions should result on INTEGERS ONLY and dividends should go from 0 to 100.
 *  [-] Users should be presented with a menu to choose an operation
 *  [-] You should record previous games in a List and there should be an option in the menu for the user to visualize a history of previous games.
 *  [-] You don't need to record results on a database. Once the program is closed the results will be deleted.
 *  ## Challenges
 *  [ ] Try to implement levels of difficulty.
 *  [ ] Add a timer to track how long the user takes to finish the game.
 *  [ ] [WIP] Add a function that let's the user pick the number of questions.
 *  [-] Create a 'Random Game' option where the players will be presented with questions from random operations
 */

using System.Data;

class Program
{
    static void Main(string[] args)
    {
        Console.BackgroundColor = ConsoleColor.White;
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Black;

        new MathGame().Run();
    }
}

class MathGame
{
    enum Operation
    {
        Addition = 1,
        Substraction = 2,
        Multiplication = 3,
        Division = 4,
    }

    private bool isEndGame = false;
    private Operation selectedOperation;
    int[] operands = new int[2];
    int userAnswer = 0;
    int answer;
    List<string> gameHistory = new();

    public void Run()
    {
        Console.WriteLine("---Welcome to Math Game---");
        while (!isEndGame)
        {
            PromptMenu();
        }
    }

    private void PromptMenu()
    {
        Console.WriteLine("Please select operation from the list");
        Console.WriteLine("0 - Exit game");
        var operationList = Enum.GetValues(typeof(Operation));
        foreach (Operation operation in operationList)
        {
            Console.WriteLine($"{(int)operation} - {operation}");
        }
        Console.WriteLine($"5 - View previous game history");
        Console.WriteLine($"6 - Give me some random game!");

        bool isValidInput = false;
        int input = 0;
        selectedOperation = Operation.Addition;

        while (!isValidInput)
        {
            Console.Write("\nYour option: ");

            try
            {
                input = Convert.ToInt32(Console.ReadLine());

                if (input < 0 || input > 6)
                {
                    throw new Exception("");
                }

                if(input == 0)
                {
                    isEndGame = true;
                    return;
                }

                isValidInput = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Please enter a correct option (0-5)");
                continue;
            }
        }

        if(input == 5)
        {
            ShowHistory();
        }
        else if(input == 6)
        {
            selectedOperation = (Operation)(new Random().Next(1, operationList.Length));
            PromptQuestion();
        }
        else
        {
            selectedOperation = (Operation)input;
            PromptQuestion();
        }
    }

    private void ShowHistory()
    {
        Console.WriteLine("\n--- Game History ---");
        if(gameHistory.Count < 1)
        {
            Console.WriteLine("You don't have any game history yet");
        }
        foreach (var entry in gameHistory)
        {
            Console.WriteLine($"{entry}\n");
        }
        Console.WriteLine("\nPress any key to go back");
        Console.ReadKey();
        Console.Clear();
    }

    private void PromptQuestion()
    {
        Random random = new();
        operands[0] = random.Next(0, 100 + 1);
        operands[1] = random.Next(1, 10 + 1);

        string operatorSymbol = GetSymbol(selectedOperation);

        Console.WriteLine("\nAnswer the question below");
        Console.WriteLine($"{operands[0]} {operatorSymbol} {operands[1]}");
        Console.Write("\nYour answer: ");

        var isValidInput = false;

        while (!isValidInput)
        {
            try
            {
                userAnswer = Convert.ToInt32(Console.ReadLine());
                isValidInput = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid input, please enter numeric value");
                continue;
            }
        }

        Console.WriteLine();

        CheckAnswer();
    }

    private void CheckAnswer ()
    {
        answer = GetAnswer(operands, selectedOperation);

        if (userAnswer == answer)
        {
            Console.WriteLine($"You got it right! the answer is {answer}");
        }
        else
        {
            Console.WriteLine($"Ops! {userAnswer} is wrong answer, the correct answer is {answer}");
        }

        AddToHistory();

        Console.WriteLine("\nPress any key for new game or 'Esc' key to exit");
        isEndGame = Console.ReadKey().Key == ConsoleKey.Escape;
        Console.Clear();

    }

    private void AddToHistory()
    {
        string record = $"Question: {operands[0]} {GetSymbol(selectedOperation)} {operands[1]}";
        record += $"\nYour answer: {userAnswer}";
        record += $"\nCorrect answer: {answer}";
        record += $"\nResult: {(answer == userAnswer ? "Correct" : "Wrong")}";

        gameHistory.Add(record);
    }

    private string GetSymbol(Operation operation)
    {
        switch (operation)
        {
            case Operation.Substraction:
                return "-";
            case Operation.Multiplication:
                return "x";
            case Operation.Division:
                return "/";
            case Operation.Addition:
            default:
                return "+";
        }
    }

    private int GetAnswer(int[] operands, Operation operation)
    {
        switch (operation)
        {
            case Operation.Substraction:
                return operands[0] - operands[1];
            case Operation.Multiplication:
                return operands[0] * operands[1];
            case Operation.Division:
                return operands[0] / operands[1];
            case Operation.Addition:
            default:
                return operands[0] + operands[1];
        }
    }
}