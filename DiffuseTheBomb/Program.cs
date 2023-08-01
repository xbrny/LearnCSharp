/*
 * The Diffuse the bomb game is about a math challenge.
 * This challenge revolves around us as contestants resolving a randomly generated math question within a set given
 * time frame. In other words, What is 2 + 2? Think fast!
 * And try not to cave into the pressure set by a 10-second asynchronously running timer.
 */

class Program
{
    private enum Operator
    {
        Sum,
        Minus,
        Multiply,
        Divide
    };

    private record Puzzle(string Question, string Answer)
    {
        public string Evaluate(string usersAnswer)
        {
            return usersAnswer switch
            {
                "" => "You didn't answer! Disqualified!",
                _ when usersAnswer == Answer => $"You Won! Your answer of {usersAnswer} was correct!",
                _ => $"The correct answer is {Answer}, but you entered: {usersAnswer}! Disqualified!"
            };
        }
    }

    private static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Diffuse the Bomb! Here is your question!");
        var task = RunTheGame();
        task.Wait();

        static async Task RunTheGame()
        {
            var cancellationToken = new CancellationTokenSource();
            string finalMessage = await await Task.WhenAny(Task.Run(PuzzleApp, cancellationToken.Token), ActivateBombAsync(cancellationToken.Token));
            cancellationToken.Cancel();
            Console.Clear();
            Console.WriteLine($"\r{finalMessage}");
        }

        static async Task<string> ActivateBombAsync(CancellationToken cancellationToken)
        {
            for (var i = 1000; i >= 0; i--)
            {
                await Task.Delay(1, cancellationToken);
                Console.Write("\r {0}:{1:F0} <- Answer quick", i / 100, (i % 100));
            }

            return "Timer runs out, Game over!";
        }

        static string PuzzleApp()
        {
            static Puzzle GenerateAPuzzle()
            {
                var (left, operation, right) = GenerateEquation();

                static Puzzle CreatePuzzle(int left, Operator operation, int right) => operation switch
                {
                    Operator.Sum => new Puzzle($"{left} + ? = {left + right}", $"{right}"),
                    Operator.Minus => new Puzzle($"{left} - ? = {left - right}", $"{right}"),
                    Operator.Multiply => new Puzzle($"{left} x ? = {left * right}", $"{right}"),
                    Operator.Divide => new Puzzle($"{left} / ? = {left / right}", $"{right}"),
                    _ => throw new ArgumentOutOfRangeException($"{typeof(Operator)}: {operation}")
                };

                var puzzle = CreatePuzzle(left, operation, right);

                return puzzle;
            }

            var puzzle = GenerateAPuzzle();
            Console.WriteLine($"\r{puzzle.Question}");
            var usersAnswer = Console.ReadLine() ?? "";
            return puzzle.Evaluate(usersAnswer);
        }
    }

    private static (int left, Operator operation, int right) GenerateEquation()
    {
        var random = new Random();
        var operation = (Operator)new Random().Next(3);
        var operand1 = random.Next(1, 10);
        var operand2 = random.Next(1, 10);
        return (operand1, operation, operand2);
    }
}