namespace GuessTheWord;

/// The guess the word game, also known by its more popular name, the hangman game,is a game where an unknown word gets
/// shown in a format where only the number of individual letters is shown. The player then has to one by one guess
/// letters to hopefully uncover some of the letters contained within that word.
/// The goal is to guess the word before the available number of guesses runs out.
///
/// Steps summary:
/// - We want to visualize the number of letters our chosen word has using underscores.
/// - We want to check every position of our word with each guess to see if that guess is contained.
/// - We must keep an eye on the number of lives a player has left and subtract one on every failed guess.
/// - With every guess match, we need to reveal that letter and show that to the player.
/// - We need to cue the win scenario if each letter has been guessed.
/// - Finally, we need to cue the lose scenario if the word has not been guessed yet and no more lives are left.
internal abstract class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Guess the word game");
        Console.WriteLine("Host enter the word (Y) or random word (N)?");
        
        var isValidInput = false;
        string? userInput;
        do
        {
            Console.Write("Your option: ");
            userInput = (Console.ReadLine() ?? "").ToUpper();

            if (userInput == "Y" || userInput == "N")
            {
                isValidInput = true;
            }
            else
            {
                Console.WriteLine("Please input either \"Y\" or \"N\"");
            }
        } while (!isValidInput);

        Console.Clear();

        string? wordToGuess;
        var isValidWord = false;
        
        if (userInput == "Y")
        {
            Console.WriteLine("Host, please enter the word");

            do
            {
                Console.Write("Your word: ");
                wordToGuess = Console.ReadLine() ?? "";
                if (wordToGuess.Length > 0 && wordToGuess.All(char.IsLetter))
                {
                    isValidWord = true;
                    wordToGuess = wordToGuess.ToUpper();
                }
                else
                {
                    Console.WriteLine("Please enter letter only and make sure it's not empty");
                }
            } while (!isValidWord);
        }
        else
        {
            Console.WriteLine("You have selected N");
            string path = Path.Combine(@"..\..\..\WordToGuess.txt");
            string[] allWords = File.ReadAllLines(path);
            Random random = new();
            int randomIndex = random.Next(allWords.Length);
            wordToGuess = (allWords[randomIndex] ?? "").ToUpper();
        }

        Console.Clear();

        int wordLength = wordToGuess.Length;
        var positionsToGuess = new char[wordLength];
        for (var i = 0; i < positionsToGuess.Length; i++)
        {
            positionsToGuess[i] = '_';
        }

        var playerLives = 5;
        var letterGuessed = new List<char>();
        var gameWon = false;

        while (playerLives > 0)
        {
            var printProgress = string.Concat(positionsToGuess);
            Console.WriteLine("Word to guess {0}", printProgress);
            Console.WriteLine("You have {0} lives", playerLives);
            
            if (printProgress == wordToGuess)
            {
                gameWon = true;
                break;
            }

            string? playerGuess;
            var isValidLetter = false;
            
            do
            {
                Console.Write("\n\nGuess a letter: ");
                playerGuess = (Console.ReadLine() ?? "").ToUpper();

                if (playerGuess.Length > 0 && playerGuess.All(char.IsLetter))
                {
                    isValidLetter = true;
                }
            } while (!isValidLetter);

            char playerGuessAsChar = Convert.ToChar(playerGuess);

            bool inHistory = letterGuessed.Contains(playerGuessAsChar);

            if (!inHistory)
            {
                letterGuessed.Add(playerGuessAsChar);
                
                var letterFound = false;
                var letterFoundCount = 0;

                for (var i = 0; i < wordToGuess.Length; i++)
                {
                    if (wordToGuess[i] != playerGuessAsChar) continue;

                    positionsToGuess[i] = playerGuessAsChar;
                    letterFound = true;
                    letterFoundCount++;
                }

                Console.Clear();

                if (letterFound)
                {
                    Console.WriteLine("Found {0} letter {1}", letterFoundCount, playerGuessAsChar);
                }
                else
                {
                    Console.WriteLine("No letter {0}", playerGuessAsChar);
                    playerLives--;
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("You already guessed {0}!", playerGuessAsChar);
            }
        }

        Console.Clear();
        Console.WriteLine("The word was: {0}", wordToGuess);
        Console.WriteLine(gameWon ? "You Won!" : "You Lose..");
    }
}