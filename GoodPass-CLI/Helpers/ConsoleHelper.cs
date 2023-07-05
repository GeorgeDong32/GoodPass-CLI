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

    /// <summary>
    /// 密码行保护
    /// </summary>
    /// <param name="linebefore">保护行位置</param>
    /// <param name="mode">1：get命令显示保护，2：add命令显示时保护</param>
    public static void PasswordLineProtect(int linebefore, int mode)
    {
        switch (mode)
        {
            case 1:
                var currentLineCursor1 = Console.CursorTop;
                Console.SetCursorPosition(0, currentLineCursor1 - linebefore);
                Console.Write(" {0,-15}\t{1,-40}", "Password", "************");
                Console.SetCursorPosition(0, currentLineCursor1);
                break;
            case 2:
                var currentLineCursor2 = Console.CursorTop;
                Console.SetCursorPosition(0, currentLineCursor2 - linebefore);
                Console.Write(" {0,-15}\t{1,-40}", "Password", "************");
                Console.SetCursorPosition(0, currentLineCursor2);
                break;
            default:
                var currentLineCursor3 = Console.CursorTop;
                Console.SetCursorPosition(0, currentLineCursor3 - linebefore);
                Console.Write(" [ Your password has been protected by GoodPass CLI ]");
                Console.SetCursorPosition(0, currentLineCursor3);
                break;
        }

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

    public static void BackspaceToLine(int linebefore)
    {
        var currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, currentLineCursor - linebefore);
        Console.SetCursorPosition(0, Console.CursorTop);
    }

    public static void ClearCurrentConsoleLine()
    {
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
    }

    public static string? ReadPassword()
    {
        var password = "";
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password = password.Remove(password.Length - 1);
                    Console.Write("\b \b");
                }
            }
            else
            {
                password += key.KeyChar;
                Console.Write("*");
            }
        }
        return password;
    }
}
