/* # Math game [https://www.thecsharpacademy.com/project/53]
 * ## Requirements
 *  - You need to create a Math game containing the 4 basic operations
 *  - The divisions should result on INTEGERS ONLY and dividends should go from 0 to 100.
 *  - Users should be presented with a menu to choose an operation
 *  - You should record previous games in a List and there should be an option in the menu for the user to visualize a history of previous games.
 *  - You don't need to record results on a database. Once the program is closed the results will be deleted.
 *  ## Challenges
 *  - Try to implement levels of difficulty.
 *  - Add a timer to track how long the user takes to finish the game.
 *  - Add a function that let's the user pick the number of questions.
 *  - Create a 'Random Game' option where the players will be presented with questions from random operations
 */

class Program
{
    static void Main(string[] args)
    {
        new MathGame().Run();
    }
}

class MathGame
{
    enum OperationOption
    {
        Addition = 1,
        Substraction = 2,
        Multiplication = 3,
        Division = 4,
    }

    private bool isEndGame = false;
    private OperationOption selectedOperation;
    int[] operands = new int[2];
    int userAnswer = 0;
    int answer;
    List<string> history = new();

    public void Run()
    {

        while (!isEndGame)
        {
            PromptMenu();
        }
    }

    private void PromptMenu()
    {
        Console.WriteLine("---Welcome to Math Game---");
        Console.WriteLine("Please select operation from the list");
        foreach (OperationOption item in Enum.GetValues(typeof(OperationOption)))
        {
            Console.WriteLine($"{(int)item} - {item}");
        }
        Console.WriteLine("Press 0 to exit");

        bool isValidInput = false;
        int input = 0;
        selectedOperation = OperationOption.Addition;

        while (!isValidInput)
        {
            try
            {
                input = Convert.ToInt32(Console.ReadLine());

                if (input < 0 || input > 4)
                {
                    throw new Exception("");
                }

                if(input == 0)
                {
                    isEndGame = true;
                    return;
                }

                selectedOperation = (OperationOption)input;

                isValidInput = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Please enter a correct option (1-4)");
                continue;
            }
        }

        Console.WriteLine("You have selected {0}", input);

        PromptQuestion();
    }

    private void PromptQuestion()
    {
        Random random = new();
        operands[0] = random.Next(1, 10 + 1);
        operands[1] = random.Next(1, 10 + 1);

        string operatorSymbol = GetSymbol(selectedOperation);

        Console.WriteLine("\nAnswer the question below");
        Console.WriteLine($"{operands[0]} {operatorSymbol} {operands[1]}");
        Console.WriteLine("\nYour answer: ");

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

        Console.WriteLine("\nPress any key to restart or 'Esc' key to exit");
        isEndGame = Console.ReadKey().Key == ConsoleKey.Escape;
        Console.Clear();

    }

    private void AddToHistory()
    {
        string record = $"{operands[0]} {GetSymbol(selectedOperation)} {operands[1]}";

        record += $"\nCorrect answer: {answer}, Your answered: {userAnswer}";

        record += $"\nResult: {answer == userAnswer}";

        history.Add(record);
    }

    private string GetSymbol(OperationOption operation)
    {
        switch (operation)
        {
            case OperationOption.Substraction:
                return "-";
            case OperationOption.Multiplication:
                return "x";
            case OperationOption.Division:
                return "/";
            case OperationOption.Addition:
            default:
                return "+";
        }
    }

    private int GetAnswer(int[] operands, OperationOption operation)
    {
        switch (operation)
        {
            case OperationOption.Substraction:
                return operands[0] - operands[1];
            case OperationOption.Multiplication:
                return operands[0] * operands[1];
            case OperationOption.Division:
                return operands[0] / operands[1];
            case OperationOption.Addition:
            default:
                return operands[0] + operands[1];
        }
    }
}