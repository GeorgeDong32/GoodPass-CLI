namespace GoodPass_CLI.Helpers;
public static class ConsoleHelper
{
    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public static void PrintWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public static void PrintGreen(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public static void PasswordLineProtect(int linebefore)
    {
        var currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, currentLineCursor - linebefore);
        Console.Write(" [ Your password has been protected by GoodPass CLI ]");
        Console.SetCursorPosition(0, currentLineCursor);
    }

    public static void PasswordCommandProtect(int linebefore)
    {
        var currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, currentLineCursor - linebefore);
        Console.Write(">>> [ Your password has been protected by GoodPass CLI ]");
        Console.SetCursorPosition(0, currentLineCursor);
    }

    public static void CleanConsoleLine(int lineindex)
    {
        var currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, currentLineCursor - lineindex);
        ClearCurrentConsoleLine();
        Console.SetCursorPosition(0, currentLineCursor);
    }

    public static void ClearCurrentConsoleLine()
    {
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
    }
}
