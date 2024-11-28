using System;
using System.Collections.Generic;

namespace MathGame
{
    internal class Program
    {
        static readonly List<GameRecord> gameHistory = new List<GameRecord>();
        static readonly Random random = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Math Game!");
            Console.WriteLine("- - - - - - - - - - - - - -");
            Console.WriteLine("Press any key to start the game!");
            Console.ReadKey();

            bool shouldExit = false;
            do
            {
                DisplayMenuOptions();

                int selectedOption = UserInput.GetIntegerInput(prompt: "Enter a option", reaskPrompt: "Please select an option");

                switch (selectedOption)
                {
                    case 0:
                        ShowHistory();
                        break;
                    case 1:
                        PlayGame(GameMode.Addition, (x, y) => x + y, "+");
                        break;
                    case 2:
                        PlayGame(GameMode.Subtraction, (x, y) => x - y, "-");
                        break;
                    case 3:
                        PlayGame(GameMode.Multiplication, (x, y) => x * y, "x");
                        break;
                    case 4:
                        PlayGame(GameMode.Division, (x, y) => x / y, "÷", isValid: (x, y) => y != 0 && x % y == 0);
                        break;
                    case 5:
                        GameMode randomMode = (GameMode)random.Next(0, 4); 
                        switch (randomMode)
                        {
                            case GameMode.Addition:
                                PlayGame(GameMode.Addition, (x, y) => x + y, "+");
                                break;
                            case GameMode.Subtraction:
                                PlayGame(GameMode.Subtraction, (x, y) => x - y, "-");
                                break;
                            case GameMode.Multiplication:
                                PlayGame(GameMode.Multiplication, (x, y) => x * y, "*");
                                break;
                            case GameMode.Division:
                                PlayGame(GameMode.Division, (x, y) => x / y, "/", (x, y) => y != 0 && x % y == 0);
                                break;
                        }
                        break;
                    case 6:
                        Console.WriteLine("Exiting the game. Goodbye!");
                        Console.WriteLine("Press any key to exit");
                        Console.ReadKey();
                        shouldExit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.ReadKey();
                        break;
                }
            } while (!shouldExit);
        }

        static void DisplayMenuOptions()
        {
            Console.Clear();
            Console.WriteLine("- - - - - - - - - - - - -");
            Console.WriteLine("Select a mode to start:");
            Console.WriteLine("0. Show History");
            Console.WriteLine("1. Addition");
            Console.WriteLine("2. Subtraction");
            Console.WriteLine("3. Multiplication");
            Console.WriteLine("4. Division");
            Console.WriteLine("5. Random Mode");
            Console.WriteLine("6. Quit the Game");
            Console.WriteLine("- - - - - - - - - - - - -");
        }

        static void ShowHistory()
        {
            Console.Clear();
            Console.WriteLine("Game History");
            Console.WriteLine("- - - - - - - - - - - -");

            if (gameHistory.Count > 0)
            {
                Console.WriteLine($"{"Date",-12} {"Game Mode",-15} {"Points",-8}");
                Console.WriteLine("- - - - - - - - - - - - - - - -");
                foreach (var record in gameHistory)
                {
                    Console.WriteLine($"{record.Date,-12} {record.Mode,-15} {record.Points,-8}");
                }
            }
            else
            {
                Console.WriteLine("No history available.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void PlayGame(GameMode mode, Func<int, int, int> operation, string symbol, Func<int, int, bool>? isValid = null)
        {
            int difficulty = 1;
            while (true)
            {
                Console.WriteLine($"Select difficulty: 1. Easy, 2. Medium, 3. Hard");
                difficulty = UserInput.GetIntegerInput("Difficulty");
                if (difficulty >= 1 && difficulty <= 3)
                    break;

                ShowMessage.Error("Invalid choice! Please select 1, 2, or 3.");
            }



            Console.Clear();
            Console.WriteLine($"Game Mode: {mode}");
            Console.WriteLine("- - - - - - - - - - -");

            int count = 0;
            int score = 0;

            int randomLimit = difficulty * 50;

            while (count < 5)
            {
                int num1, num2, result;
                do
                {
                    num1 = random.Next(1, randomLimit);
                    num2 = random.Next(1, randomLimit);
                } while (isValid != null && !isValid(num1, num2));

                result = operation(num1, num2);
                ShowMessage.Bright($"Que. {num1} {symbol} {num2} = ?");
                int userAnswer = UserInput.GetIntegerInput("Ans");

                if (userAnswer == result)
                {
                    ShowMessage.Success("Correct! +4 points");
                    score += 4;
                }
                else
                {
                    ShowMessage.Error("Wrong! -1 point");
                    score -= 1;
                }

                if (++count < 5)
                {
                    Console.WriteLine("Press any key for the next question...");
                    Console.ReadKey();
                }
            }

            gameHistory.Add(new GameRecord
            {
                Date = DateTime.Now.ToString("d"),
                Mode = mode.ToString(),
                Points = score
            });

            Console.WriteLine($"Your final score: {score}\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    internal enum GameMode
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

    internal class GameRecord
    {
        public string? Date { get; set; }
        public string? Mode { get; set; }
        public int Points { get; set; }
    }

    internal static class ShowMessage
    {
        private static void ChangeForeGroundColor(ConsoleColor color, string message)
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }

        public static void Success(string message) => ChangeForeGroundColor(ConsoleColor.Green, message);
        public static void Error(string message) => ChangeForeGroundColor(ConsoleColor.Red, message);
        public static void Bright(string message) => ChangeForeGroundColor(ConsoleColor.White, message);
    }

    internal static class UserInput
    {
        public static int GetIntegerInput(string prompt = "Enter", string reaskPrompt = "Please Enter", bool clearConsoleOnError = false)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int value))
                    return value;

                if (clearConsoleOnError) Console.Clear();
                Console.WriteLine("Invalid input. Please enter a valid integer.");
            }
        }

        public static string GetStringInput(string prompt, string reaskPrompt, bool clearConsoleOnError = false, int maxNum = int.MaxValue)
        {
            Console.Write($"{prompt}: ");
            string? input = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(input))
            {
                if (clearConsoleOnError) Console.Clear();
                Console.Write($"{reaskPrompt}: ");
                input = Console.ReadLine();
            }

            return input;
        }
    }
}
